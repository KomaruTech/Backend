using System.ComponentModel.DataAnnotations;

namespace TemplateService.API.Models;

public class CreateDocumentModel
{
    /// <summary>
    /// Название документа
    /// </summary>
    [Required]
    public string DocumentName { get; set; }
}
