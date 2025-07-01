using TemplateService.Application.TelegramService;

namespace TemplateService.Application.Telegram.Services;

public interface ITelegramNotificationSender
{
    Task SendEventNotificationToTgService(SendToTelegramEventDto dto, CancellationToken cancellationToken);
}