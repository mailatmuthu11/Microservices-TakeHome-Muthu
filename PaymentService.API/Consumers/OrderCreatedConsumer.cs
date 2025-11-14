using MassTransit;
using PaymentService.API.Application.Interfaces;
using Shared.Contracts.Events;

namespace PaymentService.API.Consumers;
public class OrderCreatedConsumer : IConsumer<OrderCreatedEvent>
{
    private readonly IPaymentService _paymentService;
    private readonly ILogger<OrderCreatedConsumer> _logger;

    public OrderCreatedConsumer(IPaymentService paymentService, ILogger<OrderCreatedConsumer> logger)
    {
        _paymentService = paymentService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
    {
        var msg = context.Message;
        _logger.LogInformation("Received OrderCreatedEvent for OrderId {OrderId}, Amount {Amount}", msg.OrderId, msg.Amount);

        try
        {
            // process payment using application service
            var payment = await _paymentService.ProcessPaymentAsync(msg.OrderId, msg.Amount, context.CancellationToken);
            _logger.LogInformation("Processed payment {PaymentId} for Order {OrderId}", payment.PaymentId, msg.OrderId);
        }
        catch (Exception ex)
        {
            // log and optionally do compensating actions - for now log
            _logger.LogError(ex, "Error processing payment for Order {OrderId}", msg.OrderId);
            // You might choose to throw so message goes to retry/dead-letter depending on MassTransit policy.
            throw;
        }
    }
}