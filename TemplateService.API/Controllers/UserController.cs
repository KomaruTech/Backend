using System.Net.Mime;
using MediatR;
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
public class UserController : ControllerBase
{
    private readonly IMediator _mediator;

    public UserController(IMediator mediator) => _mediator = mediator;

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserDto>> GetUser(Guid id)
    {
        var user = await _mediator.Send(new GetUserQuery(id));
        return user != null ? Ok(user) : NotFound();
    }

    
    [HttpPost]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<UserDto>> CreateUser([FromBody] CreateUserCommand command)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var createdUser = await _mediator.Send(command);

        // Возвращаем 201 Created с Location на нового пользователя
        return CreatedAtAction(nameof(GetUser), new { id = createdUser.Id }, createdUser);
    }
}

