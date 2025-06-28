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
    /// ID Создателя команды (Может ею управлять)
    /// </summary>
    public Guid OwnerId { get; set; }
    
    /// <summary>
    /// Создатель команды
    /// </summary>
    public virtual UserEntity Owner { get; set; }

    /// <summary>
    /// Промежуточная сущность для связи с пользователями
    /// </summary>
    public virtual ICollection<UserTeamsEntity> Users { get; set; } = new List<UserTeamsEntity>();
}