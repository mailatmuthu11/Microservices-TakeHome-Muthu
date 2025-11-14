using PaymentService.API.Application.Interfaces;
using PaymentService.API.Domain.Entities;
using System.Collections.Concurrent;

namespace PaymentService.API.Infrastructure.Repositories;
public class InMemoryPaymentRepository : IPaymentRepository
{
    private readonly ConcurrentDictionary<string, PaymentRecord> _store = new();

    public Task AddAsync(PaymentRecord record, CancellationToken ct = default)
    {
        _store.TryAdd(record.PaymentId, record);
        return Task.CompletedTask;
    }

    public Task<IEnumerable<PaymentRecord>> GetAllAsync(CancellationToken ct = default) =>
        Task.FromResult<IEnumerable<PaymentRecord>>(_store.Values.ToList());
}
