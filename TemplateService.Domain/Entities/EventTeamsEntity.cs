namespace TemplateService.Domain.Entities;

public class EventTeamsEntity
{
    /// <summary>
    /// Айди мероприятия
    /// </summary>
    public Guid EventId { get; set; }
    
    /// <summary>
    /// Мероприятие
    /// </summary>
    public EventEntity Event { get; set; }
    
    
    /// <summary>
    /// Айди группы
    /// </summary>
    public Guid TeamId { get; set; }
    
    /// <summary>
    /// Группа
    /// </summary>
    public TeamsEntity Team { get; set; }
}