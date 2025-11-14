using NotificationService.API.Application.Interfaces;
using NotificationService.API.Domain.Entities;
using NotificationService.API.Infrastructure.Senders;

namespace NotificationService.API.Application.Services;
public class NotificationService : INotificationService
{
    private readonly INotificationRepository _repo;
    private readonly INotificationSender _sender;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(INotificationRepository repo, INotificationSender sender, ILogger<NotificationService> logger)
    {
        _repo = repo;
        _sender = sender;
        _logger = logger;
    }

    public async Task<NotificationRecord> CreateAndSendAsync(string orderId, string message, string recipient, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(orderId)) throw new ArgumentException("orderId required", nameof(orderId));
        if (string.IsNullOrWhiteSpace(recipient)) throw new ArgumentException("recipient required", nameof(recipient));

        var id = Guid.NewGuid().ToString();
        var sentAt = DateTime.UtcNow;
        var record = new NotificationRecord(id, orderId, message, sentAt);
                
        await _repo.AddAsync(record, ct);

        try
        {
            await _sender.SendAsync(recipient, $"Payment succeeded for order {orderId}", message, ct);
            _logger.LogInformation("Notification sent for Order {OrderId} NotificationId {NotificationId}", orderId, id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send notification for Order {OrderId}", orderId);
        }

        return record;
    }

    public Task<IEnumerable<NotificationRecord>> GetAllAsync(CancellationToken ct = default) => _repo.GetAllAsync(ct);
}
