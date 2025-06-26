using System.Net.Mime;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TemplateService.Application.Document.Queries;
using TemplateService.Application.User.DTOs;
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

    //создать контроллер авторизации
}

