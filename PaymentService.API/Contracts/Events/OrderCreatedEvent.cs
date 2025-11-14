namespace PaymentService.API.Contracts.Events;

public sealed record OrderCreatedEvent(string OrderId, decimal Amount, string CustomerEmail, DateTime CreatedAt);
