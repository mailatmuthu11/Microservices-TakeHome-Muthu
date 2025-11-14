using PaymentService.API.Domain.Entities;

namespace PaymentService.API.Application.Interfaces;
public interface IPaymentService
{
    Task<PaymentRecord> ProcessPaymentAsync(string orderId, decimal amount, CancellationToken ct = default);
    Task<IEnumerable<PaymentRecord>> GetAllAsync(CancellationToken ct = default);
}
