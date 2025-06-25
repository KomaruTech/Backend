namespace TemplateService.Domain.Entities;

public class EventParticipantEntity
{
    /// <summary>
    /// ID пользователя
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// ID мероприятия
    /// </summary>
    public Guid EventId { get; set; }

    /// <summary>
    /// Пользователь
    /// </summary>
    public virtual UserEntity User { get; set; }

    /// <summary>
    /// Мероприятие
    /// </summary>
    public virtual EventEntity Event { get; set; }

    /// <summary>
    /// Является ли пользователь спикером
    /// </summary>
    public bool IsSpeaker { get; set; }

    /// <summary>
    /// Посетил ли пользователь мероприятие по итогу
    /// </summary>
    public bool AttendanceMarked { get; set; }
}