using MassTransit;
using Shared.Contracts.Events;

namespace OrderService.API.Infrastructure.Messaging;
public class MassTransitRabbitMqPublisher : IRabbitMqPublisher
{
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<MassTransitRabbitMqPublisher> _logger;

    public MassTransitRabbitMqPublisher(IPublishEndpoint publishEndpoint, ILogger<MassTransitRabbitMqPublisher> logger)
    {
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }

    public async Task PublishOrderCreatedAsync(OrderCreatedEvent orderCreated, CancellationToken ct = default)
    {
        // add correlation or headers here if needed
        _logger.LogDebug("Publishing OrderCreatedEvent for {OrderId}", orderCreated.OrderId);
        await _publishEndpoint.Publish(orderCreated, ct);
    }
}
