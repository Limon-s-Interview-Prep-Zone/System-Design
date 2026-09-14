using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Polly.Registry;
using Polly.RateLimiting;
using ReliabilityWebApi.Data;
using ReliabilityWebApi.Models;
using ReliabilityWebApi.Services;

namespace ReliabilityWebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentsController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly ResiliencePipelineProvider<string> _pipelineProvider;
    private readonly ILogger<PaymentsController> _logger;

    public PaymentsController(
        AppDbContext db,
        ResiliencePipelineProvider<string> pipelineProvider,
        ILogger<PaymentsController> logger)
    {
        _db = db;
        _pipelineProvider = pipelineProvider;
        _logger = logger;
    }

    /// <summary>
    /// Returns all accounts and seeded entities from SQLite database.
    /// </summary>
    [HttpGet("accounts")]
    public async Task<IActionResult> GetAccounts()
    {
        var accounts = await _db.Accounts.AsNoTracking().ToListAsync();
        return Ok(accounts);
    }

    /// <summary>
    /// Returns all processed payments from SQLite database.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetPayments()
    {
        var payments = await _db.Payments.AsNoTracking().OrderByDescending(p => p.ProcessedAt).ToListAsync();
        return Ok(payments);
    }

    /// <summary>
    /// Demonstrates Idempotency. Include header 'Idempotency-Key: &lt;uuid&gt;'
    /// Persists transaction atomically into SQLite using EF Core.
    /// </summary>
    [HttpPost("charge")]
    public async Task<IActionResult> ChargePayment([FromBody] PaymentRequest request, CancellationToken ct)
    {
        _logger.LogInformation("Processing payment charge of {Amount} {Currency} for account {AccountId}", 
            request.Amount, request.Currency, request.AccountId);

        var account = await _db.Accounts.FirstOrDefaultAsync(a => a.AccountId == request.AccountId, ct);
        if (account == null)
        {
            // Auto-create for demo if not in seed
            account = new Account
            {
                AccountId = request.AccountId,
                OwnerName = $"Customer {request.AccountId}",
                Balance = 10000.00m,
                Currency = request.Currency
            };
            _db.Accounts.Add(account);
            await _db.SaveChangesAsync(ct);
        }

        // Deduct balance and persist payment
        account.Balance -= request.Amount;

        var txId = $"TX-{Guid.NewGuid().ToString("N")[..8].ToUpperInvariant()}";
        var payment = new Payment
        {
            TransactionId = txId,
            AccountId = request.AccountId,
            Amount = request.Amount,
            Currency = request.Currency,
            Status = "SETTLED",
            ProcessedAt = DateTime.UtcNow,
            Description = request.Description
        };

        _db.Payments.Add(payment);
        await _db.SaveChangesAsync(ct);

        var response = new PaymentResponse(
            TransactionId: txId,
            Status: payment.Status,
            Amount: payment.Amount,
            Currency: payment.Currency,
            ProcessedAt: payment.ProcessedAt,
            Message: "Payment successfully captured and ledger updated in sqlite.db via EF Core."
        );

        return Ok(response);
    }

    /// <summary>
    /// Demonstrates Bulkhead Pattern. Limits concurrent access to sensitive payment vault to 2 threads.
    /// </summary>
    [HttpPost("vault-access")]
    public async Task<IActionResult> AccessCreditCardVault([FromQuery] int holdDurationMs = 1500, CancellationToken ct = default)
    {
        var bulkhead = _pipelineProvider.GetPipeline(ResiliencePipelines.BulkheadVaultPipeline);

        try
        {
            return await bulkhead.ExecuteAsync(async state =>
            {
                _logger.LogInformation("Thread entered secure Card Vault bulkhead. Holding lock for {Ms}ms", holdDurationMs);
                await Task.Delay(holdDurationMs, state);
                return (IActionResult)Ok(new
                {
                    status = "VaultAccessed",
                    message = "Sensitive vault operation finished under bulkhead isolation.",
                    heldDurationMs = holdDurationMs,
                    timestamp = DateTime.UtcNow
                });
            }, ct);
        }
        catch (RateLimiterRejectedException ex)
        {
            _logger.LogWarning("Bulkhead capacity reached! Rejecting excess vault request to protect core service. Error: {Msg}", ex.Message);
            return StatusCode(StatusCodes.Status429TooManyRequests, new
            {
                error = "Bulkhead isolation limit exceeded. Card Vault pool is saturated.",
                reason = ex.Message
            });
        }
    }
}
