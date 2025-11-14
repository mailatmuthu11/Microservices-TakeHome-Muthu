namespace Shared.Contracts.Events;

public record OrderCreatedEvent(
    string OrderId,
    decimal Amount,
    string CustomerEmail,
    DateTime CreatedAt
);
