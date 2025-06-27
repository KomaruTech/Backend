using System.Net.Mime;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TemplateService.Application.EventParticipant.Queries;
using TemplateService.Application.Teams.Dtos;
using TemplateService.Application.User.Queries;


namespace TemplateService.API.Controllers;

[ApiController]
[Produces(MediaTypeNames.Multipart.ByteRanges)]
[Route("api/v1/[controller]")]
[Authorize]
public class TeamsController : ControllerBase
{
    private readonly IMediator _mediator;

    public TeamsController(IMediator mediator) => _mediator = mediator;

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(TeamsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TeamsDto>> GetTeams(Guid id)
    {
        var teamsObj = await _mediator.Send(new GetTeamsQuery(id));
        return teamsObj != null ? Ok(teamsObj) : NotFound();
    }

    [HttpPost]
    [ProducesResponseType(typeof(TeamsDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TeamsDto>> CreateTeams([FromBody] GetTeamsQuery command)
    {
        var createdTeams = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetTeamsQuery), new { id = createdTeams.Id }, createdTeams);
    }
}