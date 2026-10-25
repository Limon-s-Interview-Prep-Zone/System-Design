using Microsoft.Extensions.Diagnostics.HealthChecks;
using ReliabilityWebApi.Services;

namespace ReliabilityWebApi.HealthChecks;

public class DownstreamHealthCheck : IHealthCheck
{
    private readonly ChaosSimulationState _chaosState;

    public DownstreamHealthCheck(ChaosSimulationState chaosState)
    {
        _chaosState = chaosState;
    }

    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context, 
        CancellationToken cancellationToken = default)
    {
        if (_chaosState.SimulateDownstreamOutage)
        {
            return Task.FromResult(HealthCheckResult.Unhealthy(
                "Downstream dependency is down (simulated outage). Node should be pulled from load balancer rotation."));
        }

        if (_chaosState.FailureRatePercent > 20.0)
        {
            return Task.FromResult(HealthCheckResult.Degraded(
                $"Downstream dependency experiencing high failure rate ({_chaosState.FailureRatePercent}%)."));
        }

        return Task.FromResult(HealthCheckResult.Healthy(
            "Downstream dependencies are operating within normal parameters."));
    }
}
