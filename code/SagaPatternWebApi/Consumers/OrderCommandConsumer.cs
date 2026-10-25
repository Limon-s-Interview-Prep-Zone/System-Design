using MassTransit;
using Microsoft.EntityFrameworkCore;
using SagaPatternWebApi.Contracts;
using SagaPatternWebApi.Data;

namespace SagaPatternWebApi.Consumers;

public class OrderCommandConsumer(AppDbContext dbContext, ILogger<OrderCommandConsumer> logger) :
    IConsumer<SubmitOrderCommand>,
    IConsumer<CompleteOrderCommand>,
    IConsumer<CancelOrderCommand>
{
    public async Task Consume(ConsumeContext<SubmitOrderCommand> context)
    {
        var msg = context.Message;
        logger.LogInformation("OrderService: Received SubmitOrderCommand for OrderId: {OrderId}", msg.OrderId);

        var order = new Order
        {
            Id = msg.OrderId,
            CustomerId = msg.CustomerId,
            Amount = msg.Amount,
            Quantity = msg.Quantity,
            Status = "Created",
            CreatedAtUtc = DateTime.UtcNow
        };

        await dbContext.Orders.AddAsync(order);
        await dbContext.SaveChangesAsync();

        logger.LogInformation("OrderService: Created Order in DB. Publishing OrderCreatedEvent for OrderId: {OrderId}", msg.OrderId);
        await context.Publish(new OrderCreatedEvent(msg.OrderId, msg.CustomerId, msg.Amount, msg.Quantity));
    }

    public async Task Consume(ConsumeContext<CompleteOrderCommand> context)
    {
        var orderId = context.Message.OrderId;
        logger.LogInformation("OrderService: Received CompleteOrderCommand for OrderId: {OrderId}", orderId);

        var order = await dbContext.Orders.FirstOrDefaultAsync(o => o.Id == orderId);
        if (order != null)
        {
            order.Status = "Completed";
            order.CompletedAtUtc = DateTime.UtcNow;
            await dbContext.SaveChangesAsync();
            logger.LogInformation("OrderService: Successfully completed OrderId: {OrderId}", orderId);
        }

        await context.Publish(new OrderCompletedEvent(orderId));
    }

    public async Task Consume(ConsumeContext<CancelOrderCommand> context)
    {
        var msg = context.Message;
        logger.LogWarning("OrderService: Received CancelOrderCommand for OrderId: {OrderId}. Reason: {Reason}", msg.OrderId, msg.Reason);

        var order = await dbContext.Orders.FirstOrDefaultAsync(o => o.Id == msg.OrderId);
        if (order != null)
        {
            order.Status = "Cancelled";
            order.FailureReason = msg.Reason;
            await dbContext.SaveChangesAsync();
            logger.LogInformation("OrderService: Cancelled OrderId: {OrderId}", msg.OrderId);
        }

        await context.Publish(new OrderCancelledEvent(msg.OrderId, msg.Reason));
    }
}
