using System.Net.Mime;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TemplateService.Application.Event.Commands;
using TemplateService.Application.EventParticipant.Dtos;
using TemplateService.Application.EventParticipant.Queries;
namespace TemplateService.API.Controllers;

[ApiController]
[Produces(MediaTypeNames.Application.Json)]
[Route("api/v1/[controller]")]
[Authorize]
public class EventParticipantController : ControllerBase
{
    private readonly IMediator _mediator;

    public EventParticipantController(IMediator mediator) => _mediator = mediator;

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(EventParticipantDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EventParticipantDto>> CreateParticipantEvent(Guid id)
    {
        var eventObj = await _mediator.Send(new GetEventParticipantQuery(id));
        return eventObj != null ? Ok(eventObj) : NotFound();
    }
    
    [HttpPost]
    [ProducesResponseType(typeof(EventParticipantDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<EventParticipantDto>> CreateParticipantEvent([FromBody] CreateEventCommand command)
    {
        var createdParticipantEvent = await _mediator.Send(command);
        return CreatedAtAction(nameof(CreateParticipantEvent), new { id = createdParticipantEvent.Id }, createdParticipantEvent);
    }
}