using OrderService.API.Domain.Entities;

namespace OrderService.API.Application.Interfaces;
public interface IOrderRepository
{
    Task AddAsync(Order order, CancellationToken ct = default);
    Task<IEnumerable<Order>> GetAllAsync(CancellationToken ct = default);
}
