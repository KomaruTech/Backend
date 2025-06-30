using System.Net.Mime;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TemplateService.Application.EventFeedback.DTOs;
using TemplateService.Application.EventFeedback.Queries;

namespace TemplateService.API.Controllers;
[ApiController]
[Produces(MediaTypeNames.Application.Json)]
[Route("api/v1/[controller]")]
[Authorize]
public class EventFeedbackController : ControllerBase
{
    private readonly IMediator _mediator;

    public EventFeedbackController(IMediator mediator) => _mediator = mediator;

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(EventFeedbackDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EventFeedbackDto>> GetEventFeedback(Guid id)
    {
        var eventFeedbackObj = await _mediator.Send(new GetEventFeedbackQuery(id));
        return eventFeedbackObj != null ? Ok(eventFeedbackObj) : NotFound();
    }

    [HttpPost]
    [ProducesResponseType(typeof(EventFeedbackDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<EventFeedbackDto>> CreateEventFeedback([FromBody] GetEventFeedbackQuery command)
    {
        var createdEvent = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetEventFeedbackQuery), new { id = createdEvent.Id }, createdEvent);
    }
}