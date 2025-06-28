using System.Net.Mime;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TemplateService.Application.NotificationPreferences.Dtos;
using TemplateService.Application.NotificationPreferences.Queries;


namespace TemplateService.API.Controllers;

[ApiController]
[Produces(MediaTypeNames.Multipart.ByteRanges)]
[Route("api/v1/[controller]")]
[Authorize]
public class NotificationPrefencesController : ControllerBase
{
    private readonly IMediator _mediator;

    public NotificationPrefencesController(IMediator mediator) => _mediator = mediator;

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(NotificationPreferencesDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<NotificationPreferencesDto>> GetEventPhoto(Guid id)
    {
        var notificationPreferencesObj = await _mediator.Send(new GetNotificationPreferencesQuery(id));
        return notificationPreferencesObj != null ? Ok(notificationPreferencesObj) : NotFound();
    }

    [HttpPost]
    [ProducesResponseType(typeof(NotificationPreferencesDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<NotificationPreferencesDto>> CreateEventPhoto([FromBody] GetNotificationPreferencesQuery command)
    {
        var createdNotificationPreferences = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetNotificationPreferencesQuery), new { id = createdNotificationPreferences.Id }, createdNotificationPreferences);
    }
}