using Microsoft.AspNetCore.Mvc;
using TemplateService.Telegram.DTO;
using TemplateService.Telegram.Services;
namespace TemplateService.Telegram.Controllers;

[ApiController]
[Route("api/v1/telegram/notifications")]
public class NotificationController : ControllerBase
{
    private readonly INotificationService _notificationService;

    public NotificationController(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    [HttpPost("send")]
    public async Task<IActionResult> SendNotification([FromBody] SendToTelegramEventDto dto, CancellationToken cancellationToken)
    {
        await _notificationService.SendNotification(dto, cancellationToken);
        return Ok();
    }
}