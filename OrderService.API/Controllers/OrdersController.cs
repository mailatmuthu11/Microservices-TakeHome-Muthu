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
            _logger.LogWarning(ex, "Validation failed for creating order.");
            return BadRequest(new { error = ex.Message });
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Order creation request was canceled.");
            return StatusCode(499, "Request was canceled.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while creating the order.");
            return StatusCode(500, "An internal server error occurred. Please try again later.");
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        try
        {
            var orders = await _orderService.GetAllAsync(ct);
            if (orders == null || !orders.Any())
            {
                return NotFound("No orders found.");
            }
            return Ok(orders);
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("GetAll orders request was canceled.");
            return StatusCode(499, "Request was canceled.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while fetching orders.");
            return StatusCode(500, "An internal server error occurred. Please try again later.");
        }
    }
}
