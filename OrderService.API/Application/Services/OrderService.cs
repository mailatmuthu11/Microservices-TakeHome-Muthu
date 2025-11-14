using OrderService.API.Application.Dtos;
using OrderService.API.Application.Interfaces;
using Shared.Contracts.Events;
using OrderService.API.Domain.Entities;
using OrderService.API.Infrastructure.Messaging;

namespace OrderService.API.Application.Services;
public class OrderService : IOrderService
{
    private readonly IOrderRepository _repo;
    private readonly IRabbitMqPublisher _publisher;
    private readonly ILogger<OrderService> _logger;

    public OrderService(IOrderRepository repo, IRabbitMqPublisher publisher, ILogger<OrderService> logger)
    {
        _repo = repo;
        _publisher = publisher;
        _logger = logger;
    }

    public async Task<Order> CreateOrderAsync(CreateOrderDto dto, CancellationToken ct = default)
    {
        if (dto.Amount <= 0) throw new ArgumentException("Amount must be greater than zero.", nameof(dto.Amount));
        if (string.IsNullOrWhiteSpace(dto.CustomerEmail)) throw new ArgumentException("CustomerEmail is required.", nameof(dto.CustomerEmail));

        var order = new Order
        {
            Amount = dto.Amount,
            CustomerEmail = dto.CustomerEmail
        };

        await _repo.AddAsync(order, ct);
        _logger.LogInformation("Order {OrderId} persisted.", order.OrderId);

        var evt = new OrderCreatedEvent(order.OrderId, order.Amount, order.CustomerEmail, order.CreatedAt);

        // Transport-specific publish via abstraction
        await _publisher.PublishOrderCreatedAsync(evt, ct);
        _logger.LogInformation("OrderCreatedEvent published for {OrderId}.", order.OrderId);

        return order;
    }

    public Task<IEnumerable<Order>> GetAllAsync(CancellationToken ct = default) => _repo.GetAllAsync(ct);
}