using System.Net.Mime;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TemplateService.Application.EventParticipant.Queries;
using TemplateService.Application.Teams.Commands;
using TemplateService.Application.Teams.Dtos;


namespace TemplateService.API.Controllers;
[ApiController]
[Produces(MediaTypeNames.Application.Json)]
[Route("api/v1/teams")] // Явное указание пути
[Authorize]
public class TeamsController : ControllerBase
{
    private readonly IMediator _mediator;

    public TeamsController(IMediator mediator) => _mediator = mediator;

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(TeamsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TeamsDto>> GetTeam(Guid id) 
    {
        var team = await _mediator.Send(new GetTeamsQuery(id));
        return team != null ? Ok(team) : NotFound();
    }

    [HttpPost]
    [ProducesResponseType(typeof(TeamsDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TeamsDto>> CreateTeam([FromBody] CreateTeamCommand command) // Используем команду
    {
        var createdTeam = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetTeam), new { id = createdTeam.Id }, createdTeam);
    }
}