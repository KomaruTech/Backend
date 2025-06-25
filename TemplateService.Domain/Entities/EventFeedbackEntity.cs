namespace TemplateService.Domain.Entities;

/// <summary>
/// Отзыв на мероприятие
/// </summary>
public class EventFeedbackEntity
{
    /// <summary>
    /// ID
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// ID того, кто создал мероприятие
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
    /// Оценка мероприятия (от 1 до 5) 
    /// </summary>
    public short Rating { get; set; }

    /// <summary>
    /// Время написания отзыва
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Комментарий к отзыву (не обязательный)
    /// </summary>
    public string? Comment { get; set; }
}