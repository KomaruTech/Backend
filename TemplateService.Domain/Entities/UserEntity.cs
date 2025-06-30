using TemplateService.Domain.Enums;

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
    /// Роль пользователя
    /// </summary>
    public UserRoleEnum Role { get; set; }

    /// <summary>
    /// Telegram Username пользователя (может быть null)
    /// </summary>
    public string? TelegramUsername { get; set; }
    
    /// <summary>
    /// Telegram ID (может быть null)
    /// </summary>
    public long? TelegramId { get; set; }

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
    /// Мета-информация о фото
    /// </summary>
    public string? AvatarMimeType { get; set; }
    
    /// <summary>
    /// Команды, созданные этим пользователем
    /// </summary>
    public virtual ICollection<TeamsEntity> CreatedTeams { get; set; } = new List<TeamsEntity>();
    
    /// <summary>
    /// Промежуточная сущность для связи с командами
    /// </summary>
    public virtual ICollection<UserTeamsEntity> Teams { get; set; } = new List<UserTeamsEntity>();
    
}