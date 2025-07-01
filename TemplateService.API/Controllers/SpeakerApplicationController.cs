using System.Net.Mime;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TemplateService.Application.EventParticipant.Queries;
using TemplateService.Application.SpeakerApplication.Dtos;


namespace TemplateService.API.Controllers;

[ApiController]
[Produces(MediaTypeNames.Multipart.ByteRanges)]
[Route("api/v1/[controller]")]
[Authorize]
public class SpeakerApplicationController : ControllerBase
{
    private readonly IMediator _mediator;

    public SpeakerApplicationController(IMediator mediator) => _mediator = mediator;

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(SpeakerApplicationsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SpeakerApplicationsDto>> GetSpeakerApplication(Guid id)
    {
        var speakerApplicationsObj = await _mediator.Send(new GetSprakerApplicationsQuery(id));
        return speakerApplicationsObj != null ? Ok(speakerApplicationsObj) : NotFound();
    }

    [HttpPost]
    [ProducesResponseType(typeof(SpeakerApplicationsDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<SpeakerApplicationsDto>> CreateEventPhoto([FromBody] GetSprakerApplicationsQuery command)
    {
        var createdSpeakerApplication = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetSprakerApplicationsQuery), new { id = createdSpeakerApplication.Id }, createdSpeakerApplication);
    }
}