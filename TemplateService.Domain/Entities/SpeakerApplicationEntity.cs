using TemplateService.Domain.Enums;

namespace TemplateService.Domain.Entities;

public class SpeakerApplicationEntity
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
    /// Тема выступления
    /// </summary>
    public string Topic { get; set; }

    /// <summary>
    /// Время написания отзыва
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Комментарий к отзыву (не обязательный)
    /// </summary>
    public string? Comment { get; set; }
    
    
    /// <summary>
    /// Статус заявки
    /// </summary>
    public ApplicationStatusEnum Status { get; set; }
}