using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SagaPatternWebApi.Contracts;
using SagaPatternWebApi.Data;

namespace SagaPatternWebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController(
    IBus bus,
    AppDbContext dbContext,
    ILogger<OrdersController> logger) : ControllerBase
{
    public record CheckoutRequest(string CustomerId, decimal Amount, int Quantity);

    /// <summary>
    /// Forward Flow (Happy Path):
    /// OrderCreated -> InventoryReserved -> PaymentProcessed -> OrderCompleted
    /// </summary>
    [HttpPost("checkout")]
    public async Task<IActionResult> Checkout([FromBody] CheckoutRequest request)
    {
        var orderId = Guid.NewGuid();
        logger.LogInformation("API: Submitting new Order {OrderId} for customer {CustomerId}", orderId, request.CustomerId);

        await bus.Publish(new SubmitOrderCommand(
            orderId,
            request.CustomerId,
            request.Amount,
            request.Quantity
        ));

        return Accepted($"/api/orders/{orderId}", new
        {
            OrderId = orderId,
            Status = "Submitted",
            Message = "Order submitted to Saga Orchestrator. Poll /api/orders/{orderId} to observe state transitions."
        });
    }

    /// <summary>
    /// Failure & Compensation Flow (Payment Fails):
    /// OrderCreated -> InventoryReserved -> PaymentFailed -> ReleaseInventory (Compensate) -> CancelOrder
    /// (Triggered when Amount > 1000)
    /// </summary>
    [HttpPost("checkout/fail-payment")]
    public async Task<IActionResult> CheckoutFailPayment([FromQuery] string customerId = "bob_failure_test", [FromQuery] decimal amount = 1500m)
    {
        var orderId = Guid.NewGuid();
        logger.LogInformation("API: Initiating failing payment test for Order {OrderId} ($1500)", orderId);

        await bus.Publish(new SubmitOrderCommand(
            orderId,
            customerId,
            amount, // Exceeds $1000 threshold, triggers PaymentFailedEvent
            Quantity: 2
        ));

        return Accepted($"/api/orders/{orderId}", new
        {
            OrderId = orderId,
            Flow = "Failure & Compensation Flow",
            ExpectedBehavior = "Inventory will be reserved, then Payment will fail ($1500 > $1000), triggering ReleaseInventory and CancelOrder.",
            CheckStatusUrl = $"/api/orders/{orderId}"
        });
    }

    /// <summary>
    /// Failure Flow (Inventory Out of Stock):
    /// OrderCreated -> InventoryReservationFailed -> CancelOrder
    /// (Triggered when Quantity > 10)
    /// </summary>
    [HttpPost("checkout/fail-inventory")]
    public async Task<IActionResult> CheckoutFailInventory([FromQuery] string customerId = "charlie_stock_test", [FromQuery] int quantity = 50)
    {
        var orderId = Guid.NewGuid();
        logger.LogInformation("API: Initiating out-of-stock test for Order {OrderId} (Qty: {Qty})", orderId, quantity);

        await bus.Publish(new SubmitOrderCommand(
            orderId,
            customerId,
            Amount: 100m,
            Quantity: quantity // Exceeds stock limit (10)
        ));

        return Accepted($"/api/orders/{orderId}", new
        {
            OrderId = orderId,
            Flow = "Inventory Out-of-Stock Flow",
            ExpectedBehavior = "Inventory reservation will immediately fail (Qty > 10), transitioning Saga to Cancelled.",
            CheckStatusUrl = $"/api/orders/{orderId}"
        });
    }

    /// <summary>
    /// Query the business Order entity
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetOrder(Guid id)
    {
        var order = await dbContext.Orders.AsNoTracking().FirstOrDefaultAsync(o => o.Id == id);
        if (order == null)
            return NotFound(new { Error = $"Order {id} not found." });

        return Ok(order);
    }

    /// <summary>
    /// Query the internal Saga State Machine instance
    /// </summary>
    [HttpGet("{id:guid}/saga-state")]
    public async Task<IActionResult> GetSagaState(Guid id)
    {
        var sagaState = await dbContext.OrderStates.AsNoTracking().FirstOrDefaultAsync(s => s.CorrelationId == id);
        if (sagaState == null)
            return NotFound(new { Error = $"Saga state for CorrelationId {id} not found." });

        return Ok(sagaState);
    }

    /// <summary>
    /// List all orders
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> ListOrders()
    {
        var orders = await dbContext.Orders
            .AsNoTracking()
            .OrderByDescending(o => o.CreatedAtUtc)
            .Take(20)
            .ToListAsync();

        return Ok(orders);
    }
}
