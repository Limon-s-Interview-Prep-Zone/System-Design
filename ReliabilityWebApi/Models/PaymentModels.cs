namespace ReliabilityWebApi.Models;

public record PaymentRequest(
    string AccountId,
    decimal Amount,
    string Currency,
    string Description
);

public record PaymentResponse(
    string TransactionId,
    string Status,
    decimal Amount,
    string Currency,
    DateTime ProcessedAt,
    string Message
);

public class ChaosSettings
{
    public int LatencyMs { get; set; } = 0;
    public double FailureRatePercent { get; set; } = 0.0;
    public bool SimulateDownstreamOutage { get; set; } = false;
}

public record CircuitBreakerEvent(
    string EventName,
    DateTime Timestamp,
    string Details
);

public record CatalogItem(
    string Id,
    string Name,
    decimal Price,
    bool IsFromCache
);
