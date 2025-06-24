using TemplateService.Domain.Enums;

namespace TemplateService.Domain.Entities;

/// <summary>
/// Мета-описание документа
/// </summary>
public class MetaEntity
{
    /// <summary>
    /// ID
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// ID типа мета-данных
    /// </summary>
    public MetaTypeEnum MetaTypeId { get; set; }

    /// <summary>
    /// ID TestEntity
    /// </summary>
    public Guid DocumentId { get; set; }

    /// <summary>
    /// Данные
    /// </summary>
    public string Data { get; set; }

    /// <summary>
    /// Документ
    /// </summary>
    public virtual DocumentEntity Document { get; set; }

    /// <summary>
    /// Тип мета-данных
    /// </summary>
    public virtual MetaTypeEntity MetaType { get; set; }
}
