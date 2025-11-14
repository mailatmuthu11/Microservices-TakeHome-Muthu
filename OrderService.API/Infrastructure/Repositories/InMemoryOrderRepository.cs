using OrderService.API.Application.Interfaces;
using OrderService.API.Domain.Entities;
using System.Collections.Concurrent;

namespace OrderService.API.Infrastructure.Repositories;
public class InMemoryOrderRepository : IOrderRepository
{
    private readonly ConcurrentDictionary<string, Order> _orders = new();

    public Task AddAsync(Order order, CancellationToken ct = default)
    {
        _orders.TryAdd(order.OrderId, order);
        return Task.CompletedTask;
    }

    public Task<IEnumerable<Order>> GetAllAsync(CancellationToken ct = default)
    {
        return Task.FromResult<IEnumerable<Order>>(_orders.Values.ToList());
    }
}
