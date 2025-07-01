namespace TemplateService.Application.Telegram.Services;
public interface ITelegramNotificationService
{
    /// <summary>
    /// Отправить уведомление об одном событии.
    /// </summary>
    Task SendDailyNotificationAsync(CancellationToken cancellationToken);
}