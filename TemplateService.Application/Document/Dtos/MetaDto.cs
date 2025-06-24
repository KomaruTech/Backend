namespace TemplateService.Application.Document.Dtos;

/// <summary>
/// Мета-описание документа
/// </summary>
public class MetaDto
{
    /// <summary>
    /// ID типа мета-данных
    /// </summary>
    public string MetaTypeName { get; set; }

    /// <summary>
    /// Данные
    /// </summary>
    public string Data { get; set; }
}
