using PaymentService.API.Domain.Entities;
using PaymentService.API.Domain.Entities;

namespace PaymentService.API.Application.Interfaces;

public interface IPaymentRepository
{
    Task AddAsync(PaymentRecord record, CancellationToken ct = default);
    Task<IEnumerable<PaymentRecord>> GetAllAsync(CancellationToken ct = default);
}
