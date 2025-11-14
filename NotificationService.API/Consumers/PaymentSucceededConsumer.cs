using MassTransit;
using NotificationService.API.Application.Interfaces;
using Shared.Contracts.Events;

namespace NotificationService.API.Consumers;
public class PaymentSucceededConsumer : IConsumer<PaymentSucceededEvent>
{
    private readonly INotificationService _notificationService;
    private readonly ILogger<PaymentSucceededConsumer> _logger;

    public PaymentSucceededConsumer(INotificationService notificationService, ILogger<PaymentSucceededConsumer> logger)
    {
        _notificationService = notificationService;
        _logger = logger;
        _logger.LogInformation("NotificationConsumer registered for PaymentSucceededEvent");
    }

    public async Task Consume(ConsumeContext<PaymentSucceededEvent> context)
    {
        var msg = context.Message;
        _logger.LogInformation("Received PaymentSucceededEvent for Order {OrderId} Payment {PaymentId}", msg.OrderId, msg.PaymentId);

        var message = $"Payment {msg.PaymentId} for order {msg.OrderId} of amount {msg.Amount} completed at {msg.Timestamp:O}.";

        var recipient = "customer@example.com";

        try
        {
            await _notificationService.CreateAndSendAsync(msg.OrderId, message, recipient, context.CancellationToken);
            _logger.LogInformation("Notification processed for Order {OrderId}", msg.OrderId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing notification for Order {OrderId}", msg.OrderId);
            throw; // let MassTransit apply retry policy / dead-letter if configured
        }
    }
}
