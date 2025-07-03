namespace TemplateService.Application.Telegram.Services;
public interface ITelegramNotificationService
{
    /// <summary>
    /// Отправить уведомление за 1 день до события.
    /// </summary>
    Task SendDailyNotificationAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Отправить уведомление за 1 час до события.
    /// </summary>
    Task SendHourlyNotificationAsync(CancellationToken cancellationToken);
}