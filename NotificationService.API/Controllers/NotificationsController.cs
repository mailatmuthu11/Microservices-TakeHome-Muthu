using Microsoft.AspNetCore.Mvc;
using NotificationService.API.Application.Interfaces;

namespace NotificationService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotificationsController : ControllerBase
{
    private readonly INotificationService _notificationService;
    private readonly ILogger<NotificationsController> _logger;

    public NotificationsController(INotificationService notificationService, ILogger<NotificationsController> logger)
    {
        _notificationService = notificationService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        try
        {
            var notifications = await _notificationService.GetAllAsync(ct);
            if (notifications == null || !notifications.Any())
            {
                return NotFound("No notifications found.");
            }
            return Ok(notifications);
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Request was canceled.");
            return StatusCode(499, "Request was canceled.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while fetching notifications.");
            return StatusCode(500, "An internal server error occurred. Please try again later.");
        }
    }
}
