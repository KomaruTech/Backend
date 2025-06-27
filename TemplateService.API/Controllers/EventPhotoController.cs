using System.Net.Mime;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TemplateService.Application.EventPhotos.Dtos;
using TemplateService.Application.EventPhotos.Queries;


namespace TemplateService.API.Controllers;

[ApiController]
[Produces(MediaTypeNames.Multipart.ByteRanges)]
[Route("api/v1/[controller]")]
[Authorize]
public class EventPhotoController : ControllerBase
{
    private readonly IMediator _mediator;

    public EventPhotoController(IMediator mediator) => _mediator = mediator;

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(EventPhotosDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EventPhotosDto>> GetEventPhoto(Guid id)
    {
        var eventPhotoObj = await _mediator.Send(new GetEventPhotosQuery(id));
        return eventPhotoObj != null ? Ok(eventPhotoObj) : NotFound();
    }

    [HttpPost]
    [ProducesResponseType(typeof(EventPhotosDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<EventPhotosDto>> CreateEvent([FromBody] GetEventPhotosQuery command)
    {
        var createdEventPhoto = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetEventPhotosQuery), new { id = createdEventPhoto.Id }, createdEventPhoto);
    }
}