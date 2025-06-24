using TemplateService.Domain.Enums;

namespace TemplateService.Domain.Entities;

/// <summary>
/// Тип мета-данных
/// </summary>
public class MetaTypeEntity
{
    /// <summary>
    /// ID
    /// </summary>
    public MetaTypeEnum Id { get; set; }

    /// <summary>
    /// Название
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Описание
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Мета-данные
    /// </summary>
    public virtual ICollection<MetaEntity>  Metas { get; private set; } = new HashSet<MetaEntity>();
}