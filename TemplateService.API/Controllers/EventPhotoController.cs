using MediatR;
using Microsoft.AspNetCore.Mvc;
using TemplateService.Application.EventPhoto.Dtos;
using TemplateService.Application.EventPhoto.Queries;

namespace TemplateService.API.Controllers
{
    public class EventPhotoController : Controller
    {
        private readonly IMediator _mediator;

        public EventPhotoController(IMediator mediator) => _mediator = mediator;

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(EventPhotoDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<EventPhotoDto>> GetEventPhoto(Guid id)
        {
            var eventPhotoObj = await _mediator.Send(new GetEventPhotoQuery(id));
            return eventPhotoObj != null ? Ok(eventPhotoObj) : NotFound();
        }

        [HttpPost]
        [ProducesResponseType(typeof(EventPhotoDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<EventPhotoDto>> CreateEvent([FromBody] GetEventPhotoQuery command)
        {
            var createdEventPhoto = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetEventPhotoQuery), new { id = createdEventPhoto.Id }, createdEventPhoto);
        }
    }
}
