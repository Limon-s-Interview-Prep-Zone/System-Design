using MassTransit;
using SagaPatternWebApi.Contracts;

namespace SagaPatternWebApi.Consumers;

public class InventoryCommandConsumer(ILogger<InventoryCommandConsumer> logger) :
    IConsumer<ReserveInventoryCommand>,
    IConsumer<ReleaseInventoryCommand>
{
    public async Task Consume(ConsumeContext<ReserveInventoryCommand> context)
    {
        var msg = context.Message;
        logger.LogInformation("InventoryService: Received ReserveInventoryCommand for OrderId: {OrderId}, Quantity: {Quantity}", msg.OrderId, msg.Quantity);

        // Simulate Inventory Business Check: Maximum 10 items in stock
        if (msg.Quantity > 10)
        {
            logger.LogWarning("InventoryService: Out of stock for OrderId: {OrderId}. Requested: {Qty}, Available: 10", msg.OrderId, msg.Quantity);
            await context.Publish(new InventoryReservationFailedEvent(msg.OrderId, "Insufficient inventory stock (max allowed: 10)"));
            return;
        }

        logger.LogInformation("InventoryService: Successfully reserved {Quantity} units for OrderId: {OrderId}", msg.Quantity, msg.OrderId);
        await context.Publish(new InventoryReservedEvent(msg.OrderId));
    }

    public async Task Consume(ConsumeContext<ReleaseInventoryCommand> context)
    {
        var msg = context.Message;
        // COMPENSATING ACTION: Semantic undo of the previously reserved inventory
        logger.LogWarning("InventoryService [COMPENSATION]: Releasing {Quantity} units back to stock for OrderId: {OrderId}. Reason: {Reason}",
            msg.Quantity, msg.OrderId, msg.Reason);

        await context.Publish(new InventoryReleasedEvent(msg.OrderId));
    }
}
