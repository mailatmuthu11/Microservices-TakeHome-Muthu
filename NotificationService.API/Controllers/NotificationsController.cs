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
        var notifications = await _notificationService.GetAllAsync(ct);
        return Ok(notifications);
    }
}
