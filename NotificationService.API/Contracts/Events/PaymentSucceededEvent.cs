using Shared.Contracts.Events;

public sealed record PaymentSucceededEvent(string OrderId, string PaymentId, decimal Amount, DateTime Timestamp);
