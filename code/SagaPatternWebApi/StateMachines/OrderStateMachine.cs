using MassTransit;
using SagaPatternWebApi.Contracts;
using SagaPatternWebApi.Data;

namespace SagaPatternWebApi.StateMachines;

public class OrderStateMachine : MassTransitStateMachine<OrderState>
{
    // States
    public State Submitted { get; private set; } = null!;
    public State AwaitingPayment { get; private set; } = null!;
    public State Completed { get; private set; } = null!;
    public State Cancelled { get; private set; } = null!;

    // Events
    public Event<OrderCreatedEvent> OrderCreated { get; private set; } = null!;
    public Event<InventoryReservedEvent> InventoryReserved { get; private set; } = null!;
    public Event<InventoryReservationFailedEvent> InventoryReservationFailed { get; private set; } = null!;
    public Event<PaymentProcessedEvent> PaymentProcessed { get; private set; } = null!;
    public Event<PaymentFailedEvent> PaymentFailed { get; private set; } = null!;

    public OrderStateMachine()
    {
        InstanceState(x => x.CurrentState);

        // Correlate incoming events by OrderId (matches Saga CorrelationId)
        Event(() => OrderCreated, x => x.CorrelateById(m => m.Message.OrderId));
        Event(() => InventoryReserved, x => x.CorrelateById(m => m.Message.OrderId));
        Event(() => InventoryReservationFailed, x => x.CorrelateById(m => m.Message.OrderId));
        Event(() => PaymentProcessed, x => x.CorrelateById(m => m.Message.OrderId));
        Event(() => PaymentFailed, x => x.CorrelateById(m => m.Message.OrderId));

        // 1. Initial Step: When OrderCreated is published
        Initially(
            When(OrderCreated)
                .Then(context =>
                {
                    context.Saga.CorrelationId = context.Message.OrderId;
                    context.Saga.CustomerId = context.Message.CustomerId;
                    context.Saga.Amount = context.Message.Amount;
                    context.Saga.Quantity = context.Message.Quantity;
                    context.Saga.CreatedAtUtc = DateTime.UtcNow;
                })
                .Publish(context => new ReserveInventoryCommand(
                    context.Saga.CorrelationId,
                    context.Saga.Quantity
                ))
                .TransitionTo(Submitted)
        );

        // 2. While in Submitted state
        During(Submitted,
            // Inventory reservation succeeded -> request payment
            When(InventoryReserved)
                .Then(context => context.Saga.UpdatedAtUtc = DateTime.UtcNow)
                .Publish(context => new ProcessPaymentCommand(
                    context.Saga.CorrelationId,
                    context.Saga.Amount
                ))
                .TransitionTo(AwaitingPayment),

            // Inventory reservation failed -> cancel order immediately
            When(InventoryReservationFailed)
                .Then(context =>
                {
                    context.Saga.FailureReason = context.Message.Reason;
                    context.Saga.UpdatedAtUtc = DateTime.UtcNow;
                })
                .Publish(context => new CancelOrderCommand(
                    context.Saga.CorrelationId,
                    $"Inventory Failed: {context.Message.Reason}"
                ))
                .TransitionTo(Cancelled)
        );

        // 3. While in AwaitingPayment state
        During(AwaitingPayment,
            // Happy Path: Payment succeeded -> complete order
            When(PaymentProcessed)
                .Then(context =>
                {
                    context.Saga.PaymentTransactionId = context.Message.PaymentTransactionId;
                    context.Saga.UpdatedAtUtc = DateTime.UtcNow;
                })
                .Publish(context => new CompleteOrderCommand(context.Saga.CorrelationId))
                .TransitionTo(Completed),

            // Failure & Compensation Path: Payment failed -> release inventory and cancel order!
            When(PaymentFailed)
                .Then(context =>
                {
                    context.Saga.FailureReason = context.Message.Reason;
                    context.Saga.UpdatedAtUtc = DateTime.UtcNow;
                })
                // Compensation Action 1: Release previously reserved inventory
                .Publish(context => new ReleaseInventoryCommand(
                    context.Saga.CorrelationId,
                    context.Saga.Quantity,
                    $"Payment failed: {context.Message.Reason}"
                ))
                // Compensation Action 2: Cancel the order
                .Publish(context => new CancelOrderCommand(
                    context.Saga.CorrelationId,
                    $"Payment Failed: {context.Message.Reason}"
                ))
                .TransitionTo(Cancelled)
        );
    }
}
