namespace TemplateService.Domain.Entities;

/// <summary>
/// Внешний документ
/// </summary>
public class DocumentEntity
{
    /// <summary>
    /// ID
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Номер
    /// </summary>
    public int Number { get; set; }

    /// <summary>
    /// Название
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Дата создания
    /// </summary>
    public DateTime CreationDate { get; set; }

    /// <summary>
    /// Мета-описание документа
    /// </summary>
    public virtual ICollection<MetaEntity> Metas { get; set; } = new HashSet<MetaEntity>();
}
