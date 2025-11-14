using PaymentService.API.Application.Interfaces;
using Shared.Contracts.Events;
using PaymentService.API.Domain.Entities;
using PaymentService.API.Infrastructure.Messaging;

namespace PaymentService.API.Application.Services;
public class PaymentService : IPaymentService
{
    private readonly IPaymentRepository _repo;
    private readonly IRabbitMqPublisher _publisher;
    private readonly ILogger<PaymentService> _logger;

    public PaymentService(IPaymentRepository repo, IRabbitMqPublisher publisher, ILogger<PaymentService> logger)
    {
        _repo = repo;
        _publisher = publisher;
        _logger = logger;
    }

    public async Task<PaymentRecord> ProcessPaymentAsync(string orderId, decimal amount, CancellationToken ct = default)
    {
        if (amount <= 0) throw new ArgumentException("Amount must be positive.", nameof(amount));

        _logger.LogInformation("Processing payment for Order {OrderId}", orderId);
        await Task.Delay(TimeSpan.FromMilliseconds(400), ct);

        var paymentId = Guid.NewGuid().ToString();
        var record = new PaymentRecord(paymentId, orderId, amount, DateTime.UtcNow);

        await _repo.AddAsync(record, ct);
        _logger.LogInformation("Payment recorded {PaymentId} for Order {OrderId}", paymentId, orderId);

        var evt = new PaymentSucceededEvent(orderId, paymentId, amount, record.ProcessedAt);
        await _publisher.PublishPaymentSucceededAsync(evt, ct);
        _logger.LogInformation("PaymentSucceededEvent published for PaymentId {PaymentId}", paymentId);

        return record;
    }

    public Task<IEnumerable<PaymentRecord>> GetAllAsync(CancellationToken ct = default) => _repo.GetAllAsync(ct);
}
