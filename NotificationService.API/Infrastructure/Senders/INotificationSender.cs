namespace NotificationService.API.Infrastructure.Senders;
public interface INotificationSender
{
    /// <summary>
    /// Send a notification (email, sms, etc.). Implementation may be sync/async.
    /// </summary>
    Task SendAsync(string to, string subject, string body, CancellationToken ct = default);
}
