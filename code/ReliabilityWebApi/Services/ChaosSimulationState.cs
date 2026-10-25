using System.Collections.Concurrent;
using ReliabilityWebApi.Models;

namespace ReliabilityWebApi.Services;

public class ChaosSimulationState
{
    public int LatencyMs { get; set; } = 0;
    public double FailureRatePercent { get; set; } = 0.0;
    public bool SimulateDownstreamOutage { get; set; } = false;

    private readonly ConcurrentQueue<CircuitBreakerEvent> _events = new();
    private const int MaxEvents = 50;

    public void RecordEvent(string eventName, string details)
    {
        _events.Enqueue(new CircuitBreakerEvent(eventName, DateTime.UtcNow, details));
        while (_events.Count > MaxEvents && _events.TryDequeue(out _)) { }
    }

    public IEnumerable<CircuitBreakerEvent> GetRecentEvents() => _events.ToArray().Reverse();
}
