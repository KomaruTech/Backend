
using System.Net.Mime;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TemplateService.Application.Telegram.Commands;


namespace TemplateService.API.Controllers;
/// <summary>
/// Связывание ТГ и пользователя
/// </summary>
[ApiController]
[AllowAnonymous]
[Produces(MediaTypeNames.Application.Json)]
[Route("api/v1/[controller]")]
public class TelegramController : ControllerBase
{
    private readonly IMediator _mediator;

    public TelegramController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Получение Username пользователя, и его ID (только от ТГ БОТА)
    /// </summary>
    [HttpPost("connect")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ConnectTgUserNameWithId([FromBody] UserConnectCommand command)
    {
        await _mediator.Send(command);
        return Ok();
    }
}