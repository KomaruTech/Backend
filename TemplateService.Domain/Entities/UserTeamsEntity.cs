namespace TemplateService.Domain.Entities;

public class UserTeamsEntity
{
    /// <summary>
    /// ID пользователя
    /// </summary>
    public Guid UserId { get; set; }
    
    /// <summary>
    /// ID команда
    /// </summary>
    public Guid TeamId { get; set; }

    /// <summary>
    /// Пользователь
    /// </summary>
    public virtual UserEntity User { get; set; }
    
    /// <summary>
    /// Команда
    /// </summary>
    public virtual TeamsEntity Team { get; set; }
}