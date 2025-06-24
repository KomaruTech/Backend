namespace TemplateService.Domain.Entities;

/// <summary>
/// Пользователь
/// </summary>
public class UserEntity
{
    /// <summary>
    /// Идентификатор пользователя
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Логин
    /// </summary>
    public string Login { get; set; }

    /// <summary>
    /// Хэш пароля
    /// </summary>
    public string PasswordHash { get; set; }

    /// <summary>
    /// Имя
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Фамилия
    /// </summary>
    public string Surname { get; set; }

    /// <summary>
    /// Электронная почта (может быть null)
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Telegram ID (может быть null)
    /// </summary>
    public string? TelegramId { get; set; }

    /// <summary>
    /// ID настроек уведомлений (внешний ключ)
    /// </summary>
    public int NotificationPreferencesId { get; set; }

    /// <summary>
    /// Навигационное свойство на настройки уведомлений
    /// </summary>
    public virtual NotificationPreferencesEntity NotificationPreferences { get; set; }
}