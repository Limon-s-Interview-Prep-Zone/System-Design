using MassTransit;
using SagaPatternWebApi.Contracts;

namespace SagaPatternWebApi.Consumers;

public class PaymentCommandConsumer(ILogger<PaymentCommandConsumer> logger) :
    IConsumer<ProcessPaymentCommand>
{
    public async Task Consume(ConsumeContext<ProcessPaymentCommand> context)
    {
        var msg = context.Message;
        logger.LogInformation("PaymentService: Received ProcessPaymentCommand for OrderId: {OrderId}, Amount: ${Amount}", msg.OrderId, msg.Amount);

        // Simulate Payment Failure: If amount > 1000, trigger card decline / failure
        if (msg.Amount > 1000m)
        {
            logger.LogWarning("PaymentService: Payment FAILED for OrderId: {OrderId}. Amount ${Amount} exceeds credit limit", msg.OrderId, msg.Amount);
            await context.Publish(new PaymentFailedEvent(msg.OrderId, "Card declined: Amount exceeds per-transaction limit ($1,000.00)"));
            return;
        }

        // Simulate Successful Payment
        var transactionId = $"TXN-{Guid.NewGuid().ToString("N")[..8].ToUpper()}";
        logger.LogInformation("PaymentService: Successfully processed payment for OrderId: {OrderId}. TransactionId: {TxnId}", msg.OrderId, transactionId);

        await context.Publish(new PaymentProcessedEvent(msg.OrderId, transactionId));
    }
}
