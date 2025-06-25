using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using MediatR;
using TemplateService.Application.Document.Dtos;
using TemplateService.Application.Document.Queries.GetDocuments;
using TemplateService.Application.Document.Queries.GetDocument;
using TemplateService.API.Models;
using TemplateService.Application.Document.Commands.CreateDocument;
using System.ComponentModel;

namespace TemplateService.API.Controllers;

/// <summary>
/// Документы
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
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
    /// Список документов
    /// </summary>
    /// <param name="name">Название документа</param>
    /// <param name="skip">Количество пропускаемых записей</param>
    /// <param name="take">Количество записей</param>
    /// <param name="cancellationToken">Токен отмены</param>
    [HttpGet]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<DocumentDto>>> GetDocuments([FromQuery] string name, [FromQuery, DefaultValue(0)] int skip, [FromQuery, DefaultValue(20)] int take, CancellationToken cancellationToken)
    {
        var docs = await _mediator.Send(new GetDocumentsQuery(name, skip, take), cancellationToken);
        return Ok(docs);
    }

    /// <summary>
    /// Документ
    /// </summary>
    /// <param name="id">ID документа</param>
    /// <param name="cancellationToken">Токен отмены</param>
    [HttpGet("{id}")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DocumentDto>> GetDocument(Guid id, CancellationToken cancellationToken)
    {
        var doc = await _mediator.Send(new GetDocumentQuery(id), cancellationToken);

        if (doc == null)
            return NotFound();

        return Ok(doc);
    }
}

   