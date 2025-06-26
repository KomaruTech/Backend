namespace TemplateService.Domain.Entities;

/// <summary>
/// Пользователь
/// </summary>
public class UserEntity
{
    /// <summary>
    /// ID
    /// </summary>
    public Guid Id { get; set; }

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
    public string Email { get; set; }

    /// <summary>
    /// Telegram ID (может быть null)
    /// </summary>
    public string? TelegramId { get; set; }

    /// <summary>
    /// ID настроек уведомлений (внешний ключ)
    /// </summary>
    public Guid NotificationPreferencesId { get; set; }

    /// <summary>
    /// Навигационное свойство на настройки уведомлений
    /// </summary>
    public virtual NotificationPreferencesEntity NotificationPreferences { get; set; }
    
    /// <summary>
    /// Фото пользователя в виде байтов
    /// </summary>
    public byte[]? Avatar { get; set; }
    
    /// <summary>
    /// Промежуточная сущность для связи с командами
    /// </summary>
    public virtual ICollection<UserTeamsEntity> Teams { get; set; } = new List<UserTeamsEntity>();
    
}