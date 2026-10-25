namespace SagaPatternWebApi.Contracts;

public record OrderCreatedEvent(Guid OrderId, string CustomerId, decimal Amount, int Quantity);

public record InventoryReservedEvent(Guid OrderId);

public record InventoryReservationFailedEvent(Guid OrderId, string Reason);

public record PaymentProcessedEvent(Guid OrderId, string PaymentTransactionId);

public record PaymentFailedEvent(Guid OrderId, string Reason);

public record InventoryReleasedEvent(Guid OrderId);

public record OrderCancelledEvent(Guid OrderId, string Reason);

public record OrderCompletedEvent(Guid OrderId);
