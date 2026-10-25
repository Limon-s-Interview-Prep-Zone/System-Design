using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReliabilityWebApi.Data;
using ReliabilityWebApi.Models;
using ReliabilityWebApi.Services;

namespace ReliabilityWebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Route("api/simulator")]
public class DownstreamSimulatorController : ControllerBase
{
    private readonly ChaosSimulationState _chaosState;
    private readonly AppDbContext _db;
    private readonly ILogger<DownstreamSimulatorController> _logger;
    private static int _flakyCallCounter = 0;

    public DownstreamSimulatorController(
        ChaosSimulationState chaosState, 
        AppDbContext db,
        ILogger<DownstreamSimulatorController> logger)
    {
        _chaosState = chaosState;
        _db = db;
        _logger = logger;
    }

    [HttpGet("echo")]
    public async Task<IActionResult> Echo(CancellationToken ct)
    {
        await ApplyChaosAsync(ct);
        return Ok(new { status = "OK", source = "LiveDownstreamService", timestamp = DateTime.UtcNow });
    }

    [HttpGet("flaky")]
    public async Task<IActionResult> Flaky(CancellationToken ct)
    {
        await ApplyChaosAsync(ct);

        // Fails 2 times, then succeeds on 3rd attempt (perfect to test retry backoff)
        var count = Interlocked.Increment(ref _flakyCallCounter);
        if (count % 3 != 0)
        {
            _logger.LogWarning("Downstream simulator simulating transient 500 error (attempt #{Count})", count);
            return StatusCode(500, new { error = "Simulated transient downstream error", attempt = count });
        }

        return Ok(new { status = "Recovered", message = "Request succeeded on retry!", attempt = count });
    }

    [HttpGet("slow")]
    public async Task<IActionResult> Slow([FromQuery] int delayMs = 3000, CancellationToken ct = default)
    {
        _logger.LogInformation("Downstream simulator sleeping for {DelayMs}ms to test timeout...", delayMs);
        await Task.Delay(delayMs, ct);
        return Ok(new { status = "SlowResponseSuccess", delayMs });
    }

    [HttpGet("catalog")]
    public async Task<IActionResult> GetLiveCatalog(CancellationToken ct)
    {
        await ApplyChaosAsync(ct);

        var products = await _db.Products
            .Where(p => p.IsActive)
            .Select(p => new CatalogItem(p.Id, p.Name, p.Price, false))
            .ToListAsync(ct);

        return Ok(products);
    }

    private async Task ApplyChaosAsync(CancellationToken ct)
    {
        if (_chaosState.SimulateDownstreamOutage)
        {
            _logger.LogError("Downstream outage simulated! Returning 503.");
            throw new HttpRequestException("Simulated catastrophic downstream outage!");
        }

        if (_chaosState.LatencyMs > 0)
        {
            await Task.Delay(_chaosState.LatencyMs, ct);
        }

        if (_chaosState.FailureRatePercent > 0)
        {
            var roll = Random.Shared.NextDouble() * 100.0;
            if (roll < _chaosState.FailureRatePercent)
            {
                _logger.LogWarning("Simulated random failure triggered ({Roll:F1}% < {Rate:F1}%)", roll, _chaosState.FailureRatePercent);
                throw new HttpRequestException($"Simulated random failure (FailureRate: {_chaosState.FailureRatePercent}%)");
            }
        }
    }
}
