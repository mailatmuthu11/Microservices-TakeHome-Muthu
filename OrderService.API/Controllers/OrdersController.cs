using Microsoft.AspNetCore.Mvc;
using OrderService.API.Application.Dtos;
using OrderService.API.Application.Interfaces;

namespace OrderService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;
    private readonly ILogger<OrdersController> _logger;

    public OrdersController(IOrderService orderService, ILogger<OrdersController> logger)
    {
        _orderService = orderService;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateOrderDto dto, CancellationToken ct)
    {
        try
        {
            var order = await _orderService.CreateOrderAsync(dto, ct);
            return CreatedAtAction(nameof(GetAll), new { id = order.OrderId }, order);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Validation failed");
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var orders = await _orderService.GetAllAsync(ct);
        return Ok(orders);
    }
}
