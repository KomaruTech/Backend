using System.Net.Mime;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TemplateService.Application.Document.Queries;
using TemplateService.Application.User.DTOs;
using TemplateService.Application.User.Commands;
using TemplateService.Application.User.Queries;

namespace TemplateService.API.Controllers;

/// <summary>
/// Пользователи
/// </summary>
[ApiController]
[Produces(MediaTypeNames.Application.Json)]
[Route("api/v1/[controller]")]
[Authorize]
public class UserController : ControllerBase
{
    private readonly IMediator _mediator;

    public UserController(IMediator mediator) => _mediator = mediator;

    [HttpGet("{login}")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserDto>> GetUser(string login)
    {
        var user = await _mediator.Send(new GetUserQuery(login));
        return user != null ? Ok(user) : NotFound();
    }

    
    [HttpPost("create")]
    [ProducesResponseType(typeof(CreatedUserResult), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [AllowAnonymous]
    public async Task<ActionResult<CreatedUserResult>> CreateUser([FromBody] CreateUserCommand command)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var createdUser = await _mediator.Send(command);

        // Возвращаем 201 Created с Location на нового пользователя
        return CreatedAtAction(nameof(GetUser), new { login = createdUser.Login }, createdUser);
    }
    
    [HttpDelete("{login}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> DeleteEvent(string login)
    {
        await _mediator.Send(new DeleteUserQuery(login));
        return NoContent(); // 204
    }
    
    /// <summary>
    /// Изменение настроек профиля
    /// </summary>
    [HttpPatch("me/profile")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<UserDto>> UpdateEvent([FromBody] UpdateUserProfileCommand profileCommand)
    {
        var updatedUser = await _mediator.Send(profileCommand);
        return Ok(updatedUser);
    }
}

