namespace OrderService.API.Domain.Entities;
public sealed class Order
{
    public string OrderId { get; init; } = Guid.NewGuid().ToString();
    public decimal Amount { get; init; }
    public string CustomerEmail { get; init; } = null!;
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
}
