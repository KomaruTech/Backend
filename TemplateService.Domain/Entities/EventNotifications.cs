using TemplateService.Domain.Enums;

namespace TemplateService.Domain.Entities
{
    /// <summary>
    /// Отправленное уведомление по мероприятию
    /// </summary>
    public class EventNotificationsEntity
    {
        /// <summary>
        /// ID записи
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// ID пользователя, которому отправлено уведомление
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// ID мероприятия
        /// </summary>
        public Guid EventId { get; set; }

        /// <summary>
        /// Тип уведомления (например, за час, за день)
        /// </summary>
        public NotificationTypeEnum NotificationType { get; set; }
        
        /// <summary>
        /// Пользователь
        /// </summary>
        public virtual UserEntity User { get; set; }

        /// <summary>
        /// Мероприятие
        /// </summary>
        public virtual EventEntity Event { get; set; }
    }
}