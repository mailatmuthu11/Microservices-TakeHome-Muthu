

using Shared.Contracts.Events;

namespace PaymentService.API.Infrastructure.Messaging;
public interface IRabbitMqPublisher
{
    Task PublishPaymentSucceededAsync(PaymentSucceededEvent paymentSucceeded, CancellationToken ct = default);
}