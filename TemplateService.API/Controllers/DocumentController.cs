using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using MediatR;
using TemplateService.Application.Document.Dtos;
using TemplateService.Application.Document.Queries.GetDocuments;
using TemplateService.Application.Document.Queries.GetDocument;
using TemplateService.API.Models;
using TemplateService.Application.Document.Commands.CreateDocument;
using System.ComponentModel;
using Swashbuckle.AspNetCore.Annotations;

namespace TemplateService.API.Controllers;

/// <summary>
/// API для работы с документами
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
[SwaggerTag("Управление документами - создание, получение списка и деталей")]
public class DocumentController : ControllerBase
{
    private readonly ILogger<DocumentController> _logger;
    private readonly IMediator _mediator;

    public DocumentController(ILogger<DocumentController> logger, IMediator mediator)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    /// <summary>
    /// Получить список документов с пагинацией
    /// </summary>
    /// <param name="name">Фильтр по названию документа</param>
    /// <param name="skip">Количество пропускаемых записей (для пагинации)</param>
    /// <param name="take">Количество возвращаемых записей (макс. 100)</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Список документов</returns>
    [HttpGet]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(IEnumerable<DocumentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [SwaggerOperation(
        Summary = "Получение списка документов",
        Description = "Возвращает отфильтрованный и пагинированный список документов",
        OperationId = "GetDocuments")]
    public async Task<ActionResult<IEnumerable<DocumentDto>>> GetDocuments(
        [FromQuery, SwaggerParameter("Фильтр по названию", Required = false)] string name,
        [FromQuery, DefaultValue(0), SwaggerParameter("Смещение", Required = false)] int skip,
        [FromQuery, DefaultValue(20), SwaggerParameter("Лимит (макс. 100)", Required = false)] int take,
        CancellationToken cancellationToken)
    {
        if (take > 100)
            return BadRequest("Maximum take value is 100");

        var docs = await _mediator.Send(new GetDocumentsQuery(name, skip, take), cancellationToken);
        return Ok(docs);
    }

    /// <summary>
    /// Получить документ по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор документа (GUID)</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Данные документа</returns>
    [HttpGet("{id}")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(DocumentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [SwaggerOperation(
        Summary = "Получение документа по ID",
        Description = "Возвращает полные данные документа по его идентификатору",
        OperationId = "GetDocumentById")]
    public async Task<ActionResult<DocumentDto>> GetDocument(
        [SwaggerParameter("Идентификатор документа", Required = true)] Guid id,
        CancellationToken cancellationToken)
    {
        var doc = await _mediator.Send(new GetDocumentQuery(id), cancellationToken);

        if (doc == null)
            return NotFound();

        return Ok(doc);
    }

    /// <summary>
    /// Создать новый документ
    /// </summary>
    /// <param name="model">Данные для создания документа</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Созданный документ</returns>
    [HttpPost]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(DocumentDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [SwaggerOperation(
        Summary = "Создание документа",
        Description = "Создает новый документ с указанным названием",
        OperationId = "CreateDocument")]
    public async Task<ActionResult<DocumentDto>> CreateDocument(
        [FromBody, SwaggerRequestBody("Данные для создания документа", Required = true)] CreateDocumentModel model,
        CancellationToken cancellationToken)
    {
        var doc = await _mediator.Send(new CreateDocumentCommand(model.DocumentName), cancellationToken);
        return CreatedAtAction(nameof(GetDocument), new { id = doc.Id }, doc);
    }
}