namespace OrderService.API.Application.Dtos;
public sealed record CreateOrderDto(
    decimal Amount, 
    string CustomerEmail);