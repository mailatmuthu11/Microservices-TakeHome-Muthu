namespace NotificationService.API.Infrastructure.Senders;
public interface INotificationSender
{
    Task SendAsync(string to, string subject, string body, CancellationToken ct = default);
}
