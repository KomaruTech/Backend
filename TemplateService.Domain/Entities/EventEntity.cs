namespace TemplateService.Domain.Entities;

using Enums;
/// <summary>
/// Мероприятие
/// </summary>
public class EventEntity
{
    /// <summary>
    /// ID
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Название мероприятия
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Описание мероприятия
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Время начала мероприятия
    /// </summary>
    public DateTime TimeStart { get; set; }

    /// <summary>
    /// Время окончания мероприятия
    /// </summary>
    public DateTime? TimeEnd { get; set; }

    /// <summary>
    /// Тип
    /// </summary>
    public EventTypeEnum Type { get; set; }

    /// <summary>
    /// Адрес (как URL, так и физический)
    /// </summary>
    public string? Location { get; set; }
    
    /// <summary>
    /// ID того, кто создал мероприятие
    /// </summary>
    public Guid CreatedById { get; set; }
    
    /// <summary>
    /// Тот, кто создал мероприятие
    /// </summary>
    public virtual UserEntity CreatedBy { get; set;}
    
    /// <summary>
    /// Мероприятие одобрено или нет
    /// </summary>
    public EventStatusEnum Status { get; set;}
    
    /// <summary>
    /// Список ключевых слов мероприятия
    /// </summary>
    public List<string> Keywords { get; set; } = new();
    
    /// <summary>
    /// Список участников мероприятия
    /// </summary>
    public virtual ICollection<EventParticipantEntity> Participants { get; set; } = new List<EventParticipantEntity>();
    
    /// <summary>
    /// Список фото мероприятия
    /// </summary>
    public virtual ICollection<EventPhotoEntity> Photos { get; set; } = new List<EventPhotoEntity>();

    /// <summary>
    /// Список групп добавленных на мероприятие
    /// </summary>
    public virtual ICollection<EventTeamsEntity> EventTeams { get; set; } = new List<EventTeamsEntity>();
}