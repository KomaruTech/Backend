using System.Net.Mime;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TemplateService.Application.Event.Commands;
using TemplateService.Application.Event.DTOs;
using TemplateService.Application.Event.Queries;

namespace TemplateService.API.Controllers;

/// <summary>
/// Мероприятия
/// </summary>
[ApiController]
[Produces(MediaTypeNames.Application.Json)]
[Route("api/v1/[controller]")]
[Authorize]
public class EventController : ControllerBase
{
    private readonly IMediator _mediator; 

    public EventController(IMediator mediator) => _mediator = mediator;

    /// <summary>
    /// Получение мероприятия
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(EventDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EventDto>> GetEvent(Guid id)
    {
        var eventObj = await _mediator.Send(new GetEventQuery(id));
        return eventObj != null ? Ok(eventObj) : NotFound();
    }

    /// <summary>
    /// Удаление мероприятия
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> DeleteEvent(Guid id)
    {
        await _mediator.Send(new DeleteEventQuery(id));
        return NoContent(); // 204
    }

    /// <summary>
    /// Поиск мероприятий
    /// </summary>
    [HttpPost("search")]
    [ProducesResponseType(typeof(List<EventDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult<List<EventDto>>> SearchEvents([FromBody] SearchEventsQuery request)
    {
        var events = await _mediator.Send(request);
    
        if (events.Count == 0)
            return NoContent();

        return Ok(events); // 200 OK
    }

    /// <summary>
    /// Создание мероприятия (только для организаторов/администраторов)
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(EventDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<EventDto>> CreateEvent([FromBody] CreateEventCommand command)
    {
        var createdEvent = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetEvent), new { id = createdEvent.Id }, createdEvent);
    }

    /// <summary>
    /// Частичное обновление мероприятия (только создатель и администратор)
    /// </summary>
    [HttpPatch("{id:guid}")]
    [ProducesResponseType(typeof(EventDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<EventDto>> UpdateEvent(Guid id, [FromBody] UpdateEventCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest("Id in URL and body should be the same.");
        }

        var updatedEvent = await _mediator.Send(command);
        return Ok(updatedEvent);
    }

    /// <summary>
    /// Подтверждение мероприятия (только организатор и администратор)
    /// </summary>
    [HttpPost("{id:guid}/confirm")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ConfirmEvent(Guid id)
    {
        await _mediator.Send(new ConfirmEventQuery(id));
        return NoContent(); // 204
    }

    /// <summary>
    /// Предложение провести мероприятие
    /// </summary>
    [HttpPost("suggest")]
    [ProducesResponseType(typeof(EventDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<EventDto>> SuggestEvent([FromBody] SuggestEventCommand command)
    {
        var createdEvent = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetEvent), new { id = createdEvent.Id }, createdEvent);
    }
}