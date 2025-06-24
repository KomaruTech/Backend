namespace TemplateService.Domain.Entities;

/// <summary>
/// Настройки уведомлений пользователя
/// </summary>
public class NotificationPreferencesEntity
{
    /// <summary>
    /// Идентификатор
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Уведомлять в Telegram
    /// </summary>
    public bool NotifyTelegram { get; set; }

    /// <summary>
    /// Уведомлять по Email
    /// </summary>
    public bool NotifyEmail { get; set; }

    /// <summary>
    /// Напомнить за 1 день
    /// </summary>
    public bool ReminderBefore1Day { get; set; }

    /// <summary>
    /// Напомнить за 1 час
    /// </summary>
    public bool ReminderBefore1Hour { get; set; }
}