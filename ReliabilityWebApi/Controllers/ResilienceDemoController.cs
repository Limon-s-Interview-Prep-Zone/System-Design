using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using ReliabilityWebApi.Services;

namespace ReliabilityWebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ResilienceDemoController : ControllerBase
{
    private readonly IDownstreamService _downstreamService;
    private readonly ILogger<ResilienceDemoController> _logger;

    public ResilienceDemoController(
        IDownstreamService downstreamService,
        ILogger<ResilienceDemoController> logger)
    {
        _downstreamService = downstreamService;
        _logger = logger;
    }

    /// <summary>
    /// Demonstrates Retries with Exponential Backoff and Full Jitter.
    /// Calls downstream flaky endpoint that fails initially, then recovers.
    /// </summary>
    [HttpGet("retry-with-backoff")]
    public async Task<IActionResult> TestRetry(CancellationToken ct)
    {
        _logger.LogInformation("Invoking downstream flaky endpoint via Resilience Pipeline...");
        var (code, body, isFallback) = await _downstreamService.CallDownstreamWithResilienceAsync("/api/simulator/flaky", ct);

        return StatusCode(code, new
        {
            strategy = "Retry with Exponential Backoff & Full Jitter",
            isFallback,
            downstreamStatusCode = code,
            result = body
        });
    }

    /// <summary>
    /// Demonstrates Timeout & Deadline Propagation.
    /// Downstream simulates 3000ms delay, but per-try timeout is 1500ms.
    /// </summary>
    [HttpGet("timeout-demo")]
    public async Task<IActionResult> TestTimeout([FromQuery] int delayMs = 3000, CancellationToken ct = default)
    {
        _logger.LogInformation("Invoking slow downstream endpoint ({DelayMs}ms) with 1500ms timeout policy...", delayMs);
        var (code, body, isFallback) = await _downstreamService.CallDownstreamWithResilienceAsync($"/api/simulator/slow?delayMs={delayMs}", ct);

        return StatusCode(code, new
        {
            strategy = "Timeout Policy & Deadline Enforcement",
            isFallback,
            downstreamStatusCode = code,
            result = body
        });
    }

    /// <summary>
    /// Demonstrates Circuit Breaker State Transitions.
    /// Trips after 50% failure rate; fast-fails subsequent requests without network call.
    /// </summary>
    [HttpGet("circuit-breaker-demo")]
    public async Task<IActionResult> TestCircuitBreaker(CancellationToken ct)
    {
        var (code, body, isFallback) = await _downstreamService.CallDownstreamWithResilienceAsync("/api/simulator/echo", ct);

        return StatusCode(code, new
        {
            strategy = "Circuit Breaker Pattern",
            isFallback,
            statusCode = code,
            result = body
        });
    }

    /// <summary>
    /// Demonstrates Graceful Degradation / Fallback Pattern.
    /// When live catalog dependency fails, returns cached/static backup catalog.
    /// </summary>
    [HttpGet("graceful-degradation")]
    public async Task<IActionResult> TestGracefulDegradation(CancellationToken ct)
    {
        var catalog = await _downstreamService.GetCatalogWithGracefulDegradationAsync(ct);
        var isAnyDegraded = catalog.Any(x => x.IsFromCache);

        return Ok(new
        {
            strategy = "Graceful Degradation (Fallback)",
            isDegraded = isAnyDegraded,
            message = isAnyDegraded ? "Live catalog unavailable. Serving stale/fallback catalog." : "Serving live catalog data.",
            catalog
        });
    }

    /// <summary>
    /// Demonstrates Token Bucket Rate Limiting (Policy: "token-bucket-policy").
    /// Only allows 5 requests per 10 seconds. Excess requests return HTTP 429.
    /// </summary>
    [HttpGet("rate-limited")]
    [EnableRateLimiting("token-bucket-policy")]
    public IActionResult TestRateLimiter()
    {
        return Ok(new
        {
            status = "Success",
            strategy = "Token Bucket Rate Limiting",
            message = "Request accepted within rate limit allowance.",
            timestamp = DateTime.UtcNow
        });
    }
}
