using System.Net.Mime;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TemplateService.Application.Event.DTOs;
using TemplateService.Application.Event.Queries;

namespace TemplateService.API.Controllers;

/// <summary>
/// События
/// </summary>
[ApiController]
[Produces(MediaTypeNames.Application.Json)]
[Route("api/v1/[controller]")]
public class EventController : ControllerBase
{
    private readonly IMediator _mediator;

    public EventController(IMediator mediator) => _mediator = mediator;

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(EventDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EventDto>> GetUser(Guid id)
    {
        var user = await _mediator.Send(new GetEventQuery(id));
        return user != null ? Ok(user) : NotFound();
    }
}
