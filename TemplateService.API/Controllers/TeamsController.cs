using System.Net.Mime;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TemplateService.Application.Teams.Commands;
using TemplateService.Application.Teams.Dtos;
using TemplateService.Application.Teams.Queries;


namespace TemplateService.API.Controllers;

/// <summary>
/// Команды
/// </summary>
[ApiController]
[Produces(MediaTypeNames.Application.Json)]
[Route("api/v1/teams")] // Явное указание пути
[Authorize]
public class TeamsController : ControllerBase
{
    private readonly IMediator _mediator;

    public TeamsController(IMediator mediator) => _mediator = mediator;

    /// <summary>
    /// Получение команды по UUID
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(TeamsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TeamsDto>> GetTeam(Guid id)
    {
        var team = await _mediator.Send(new GetTeamsQuery(id));
        return Ok(team);
    }

    /// <summary>
    /// Создание команды
    /// </summary>
    [HttpPost("create")]
    [ProducesResponseType(typeof(TeamsDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TeamsDto>> CreateTeam([FromBody] CreateTeamCommand command)
    {
        var createdTeam = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetTeam), new { id = createdTeam.Id }, createdTeam);
    }

    /// <summary>
    /// Поиск команд по названию (всех команд)
    /// </summary>
    [HttpPost("search")]
    [ProducesResponseType(typeof(List<TeamsDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult<List<TeamsDto>>> SearchTeams([FromBody] SearchTeamsQuery command)
    {
        var teams = await _mediator.Send(command);
        
        if (teams.Count() == 0)
            return NoContent();
        return Ok(teams);
    }
    
    /// <summary>
    /// Поиск команд по названию (только тех, где человек состоит)
    /// </summary>
    [HttpPost("search_my_teams")]
    [ProducesResponseType(typeof(List<TeamsDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult<List<TeamsDto>>> SearchTeams([FromBody] SearchMyTeamsQuery command)
    {
        var teams = await _mediator.Send(command);
        
        if (teams.Count() == 0)
            return NoContent();
        return Ok(teams);
    }
    
}