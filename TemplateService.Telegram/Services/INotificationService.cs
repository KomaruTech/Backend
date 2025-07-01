using TemplateService.Telegram.DTO;

namespace TemplateService.Telegram.Services;

public interface INotificationService
{
    Task SendNotification(SendToTelegramEventDto dto, CancellationToken cancellationToken);
}