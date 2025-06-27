using System.Net.Mime;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TemplateService.Application.UserTeams.Dtos;
using TemplateService.Application.UserTeams.Queries;


namespace TemplateService.API.Controllers;

[ApiController]
[Produces(MediaTypeNames.Multipart.ByteRanges)]
[Route("api/v1/[controller]")]
[Authorize]
public class UserTeamsController : ControllerBase
{
    private readonly IMediator _mediator;

    public UserTeamsController(IMediator mediator) => _mediator = mediator;

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(UserTeamsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserTeamsDto>> GetUserTeams(Guid id)
    {
        var userTeamsObj = await _mediator.Send(new GetUserTeamsQuery(id));
        return userTeamsObj != null ? Ok(userTeamsObj) : NotFound();
    }

    [HttpPost]
    [ProducesResponseType(typeof(UserTeamsDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<UserTeamsDto>> CreateUserTeams([FromBody] GetUserTeamsQuery command)
    {
        var createdUserTeams = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetUserTeamsQuery), new { id = createdUserTeams.UserId }, createdUserTeams);
    }
}