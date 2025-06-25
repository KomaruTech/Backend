namespace TemplateService.Domain.Entities;

public class TeamsEntity
{
    /// <summary>
    /// ID
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Название команды
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Описание команды
    /// </summary>
    public string Description { get; set; }
    
    /// <summary>
    /// Промежуточная сущность для связи с пользователями
    /// </summary>
    public virtual ICollection<UserTeamsEntity> Users { get; set; } = new List<UserTeamsEntity>();
}