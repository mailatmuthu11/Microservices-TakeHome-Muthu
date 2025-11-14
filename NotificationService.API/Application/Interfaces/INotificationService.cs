using NotificationService.API.Domain.Entities;

namespace NotificationService.API.Application.Interfaces;
public interface INotificationService
{
    Task<NotificationRecord> CreateAndSendAsync(string orderId, string message, string recipient, CancellationToken ct = default);
    Task<IEnumerable<NotificationRecord>> GetAllAsync(CancellationToken ct = default);
}
