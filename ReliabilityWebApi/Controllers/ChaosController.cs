using Microsoft.AspNetCore.Mvc;
using ReliabilityWebApi.Models;
using ReliabilityWebApi.Services;

namespace ReliabilityWebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChaosController : ControllerBase
{
    private readonly ChaosSimulationState _chaosState;

    public ChaosController(ChaosSimulationState chaosState)
    {
        _chaosState = chaosState;
    }

    [HttpGet("status")]
    public IActionResult GetStatus()
    {
        return Ok(new
        {
            CurrentSettings = new
            {
                _chaosState.LatencyMs,
                _chaosState.FailureRatePercent,
                _chaosState.SimulateDownstreamOutage
            },
            RecentResilienceEvents = _chaosState.GetRecentEvents()
        });
    }

    [HttpPost("configure")]
    public IActionResult ConfigureChaos([FromBody] ChaosSettings settings)
    {
        _chaosState.LatencyMs = settings.LatencyMs;
        _chaosState.FailureRatePercent = settings.FailureRatePercent;
        _chaosState.SimulateDownstreamOutage = settings.SimulateDownstreamOutage;

        _chaosState.RecordEvent("ChaosConfigured", 
            $"Latency: {settings.LatencyMs}ms, FailureRate: {settings.FailureRatePercent}%, Outage: {settings.SimulateDownstreamOutage}");

        return Ok(new
        {
            message = "Chaos simulation settings updated successfully.",
            updatedSettings = settings
        });
    }

    [HttpPost("reset")]
    public IActionResult Reset()
    {
        _chaosState.LatencyMs = 0;
        _chaosState.FailureRatePercent = 0.0;
        _chaosState.SimulateDownstreamOutage = false;

        _chaosState.RecordEvent("ChaosReset", "All chaos and simulated downstream faults reset to normal.");

        return Ok(new { message = "Chaos settings reset to defaults (healthy)." });
    }
}
