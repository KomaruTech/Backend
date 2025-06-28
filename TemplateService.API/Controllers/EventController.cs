using System.Net.Mime;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TemplateService.Application.Event.Commands;
using TemplateService.Application.Event.DTOs;
using TemplateService.Application.Event.Queries;
namespace TemplateService.API.Controllers;

[ApiController]
[Produces(MediaTypeNames.Application.Json)]
[Route("api/v1/[controller]")]
[Authorize]
public class EventController : ControllerBase
{
    private readonly IMediator _mediator; 

    public EventController(IMediator mediator) => _mediator = mediator;

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(EventDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EventDto>> GetEvent(Guid id)
    {
        var eventObj = await _mediator.Send(new GetEventQuery(id));
        return eventObj != null ? Ok(eventObj) : NotFound();
    }
    
    [HttpGet("search_in_interval")]
    [ProducesResponseType(typeof(List<EventDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<EventDto>>> SearchInInterval([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
    {
        var events = await _mediator.Send(new SearchInIntervalQuery(startDate, endDate));
        return Ok(events);
    }

    [HttpPost]
    [ProducesResponseType(typeof(EventDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<EventDto>> CreateEvent([FromBody] CreateEventCommand command)
    {
        var createdEvent = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetEvent), new { id = createdEvent.Id }, createdEvent);
    }
}