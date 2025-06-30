namespace TemplateService.Application.TelegramService;

public interface ITelegramNotificationService
{
    /// <summary>
    /// Отправить уведомление об одном событии.
    /// </summary>
    Task SendDailyNotificationAsync(CancellationToken cancellationToken);
}