using MassTransit;
using Shared.Contracts.Events;

namespace PaymentService.API.Infrastructure.Messaging;
public class MassTransitRabbitMqPublisher : IRabbitMqPublisher
{
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<MassTransitRabbitMqPublisher> _logger;

    public MassTransitRabbitMqPublisher(IPublishEndpoint publishEndpoint, ILogger<MassTransitRabbitMqPublisher> logger)
    {
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }

    public async Task PublishPaymentSucceededAsync(PaymentSucceededEvent paymentSucceeded, CancellationToken ct = default)
    {
        _logger.LogDebug("Publishing PaymentSucceededEvent for {OrderId} PaymentId {PaymentId}",
            paymentSucceeded.OrderId, paymentSucceeded.PaymentId);

        await _publishEndpoint.Publish(paymentSucceeded, ct);
    }
}