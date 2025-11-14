namespace PaymentService.API.Domain.Entities;

public sealed record PaymentRecord(string PaymentId, string OrderId, decimal Amount, DateTime ProcessedAt);
