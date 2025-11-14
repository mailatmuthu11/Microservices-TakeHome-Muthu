using OrderService.API.Application.Dtos;
using OrderService.API.Domain.Entities;

namespace OrderService.API.Application.Interfaces;
public interface IOrderService
{
    Task<Order> CreateOrderAsync(CreateOrderDto dto, CancellationToken ct = default);
    Task<IEnumerable<Order>> GetAllAsync(CancellationToken ct = default);
}