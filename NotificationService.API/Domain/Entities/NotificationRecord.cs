namespace NotificationService.API.Domain.Entities;

public sealed record NotificationRecord(string NotificationId, string OrderId, string Message, DateTime SentAt);
