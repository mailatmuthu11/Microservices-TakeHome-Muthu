using Shared.Contracts.Events;

namespace OrderService.API.Infrastructure.Messaging;
public interface IRabbitMqPublisher
{
    Task PublishOrderCreatedAsync(OrderCreatedEvent orderCreated, CancellationToken ct = default);
}