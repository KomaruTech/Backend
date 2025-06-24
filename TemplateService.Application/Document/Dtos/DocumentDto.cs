namespace TemplateService.Application.Document.Dtos;

/// <summary>
/// Внешний документ
/// </summary>
public class DocumentDto
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
    public IEnumerable<MetaDto> Metas { get; set; }
}
