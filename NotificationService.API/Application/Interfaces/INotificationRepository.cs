using NotificationService.API.Domain.Entities;

namespace NotificationService.API.Application.Interfaces;
public interface INotificationRepository
{
    Task AddAsync(NotificationRecord record, CancellationToken ct = default);
    Task<IEnumerable<NotificationRecord>> GetAllAsync(CancellationToken ct = default);
}
