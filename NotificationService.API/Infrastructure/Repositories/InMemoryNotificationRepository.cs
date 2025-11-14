using NotificationService.API.Application.Interfaces;
using NotificationService.API.Domain.Entities;
using System.Collections.Concurrent;

namespace NotificationService.API.Infrastructure.Repositories;
public class InMemoryNotificationRepository : INotificationRepository
{
    private readonly ConcurrentDictionary<string, NotificationRecord> _store = new();

    public Task AddAsync(NotificationRecord record, CancellationToken ct = default)
    {
        _store.TryAdd(record.NotificationId, record);
        return Task.CompletedTask;
    }

    public Task<IEnumerable<NotificationRecord>> GetAllAsync(CancellationToken ct = default) =>
        Task.FromResult<IEnumerable<NotificationRecord>>(_store.Values.ToList());
}