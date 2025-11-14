using Microsoft.AspNetCore.Mvc;
using PaymentService.API.Application.Interfaces;

namespace PaymentService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentsController : ControllerBase
{
    private readonly IPaymentService _paymentService;
    private readonly ILogger<PaymentsController> _logger;

    public PaymentsController(IPaymentService paymentService, ILogger<PaymentsController> logger)
    {
        _paymentService = paymentService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        try
        {
            var payments = await _paymentService.GetAllAsync(ct);
            if (payments == null || !payments.Any())
            {
                return NotFound("No payments found.");
            }
            return Ok(payments);
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("GetAll payments request was canceled.");
            return StatusCode(499, "Request was canceled.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while fetching payments.");
            return StatusCode(500, "An internal server error occurred. Please try again later.");
        }
    }
}
