namespace TemplateService.Domain.Entities;

public class EventPhotoEntity
{
    /// <summary>
    /// ID
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// ID мероприятия, к которому прикреплена фотография
    /// </summary>
    public Guid EventId { get; set; }

    /// <summary>
    /// Мероприятие
    /// </summary>
    public virtual EventEntity Event { get; set; }

    /// <summary>
    /// Содержимое изображения
    /// </summary>
    public byte[] Image { get; set; }

    /// <summary>
    /// MIME-тип изображения
    /// </summary>
    public string MimeType { get; set; }

    /// <summary>
    /// Дата и время загрузки
    /// </summary>
    public DateTime UploadedAt { get; set; }

    /// <summary>
    /// Описание изображения (может быть null)
    /// </summary>
    public string? Description { get; set; }
}