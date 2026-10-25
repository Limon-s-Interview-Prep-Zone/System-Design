namespace SagaPatternWebApi.Contracts;

public record SubmitOrderCommand(Guid OrderId, string CustomerId, decimal Amount, int Quantity);

public record ReserveInventoryCommand(Guid OrderId, int Quantity);

public record ProcessPaymentCommand(Guid OrderId, decimal Amount);

public record ReleaseInventoryCommand(Guid OrderId, int Quantity, string Reason);

public record CancelOrderCommand(Guid OrderId, string Reason);

public record CompleteOrderCommand(Guid OrderId);
