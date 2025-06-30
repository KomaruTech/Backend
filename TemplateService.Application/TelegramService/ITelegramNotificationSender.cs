namespace TemplateService.Application.TelegramService;

public interface ITelegramNotificationSender
{
    Task SendEventNotificationToTgService(SendToTelegramEventDto dto, CancellationToken cancellationToken);
}