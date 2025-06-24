using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace TemplateService.API.Models;

/// <summary>
/// Модель запроса для создания документа
/// </summary>
[SwaggerSchema("Запрос на создание документа", Required = new[] { "DocumentName" })]
public class CreateDocumentModel
{
    /// <summary>
    /// Название документа
    /// </summary>
    /// <example>Договор аренды №1</example>
    [Required(ErrorMessage = "Document name is required")]
    [StringLength(100, ErrorMessage = "Document name cannot be longer than 100 characters")]
    [SwaggerSchema("Название документа", Format = "string")]
    public string DocumentName { get; set; }
}