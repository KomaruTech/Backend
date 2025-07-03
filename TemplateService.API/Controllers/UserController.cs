using System.Net.Mime;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TemplateService.Application.User.Commands;
using TemplateService.Application.User.Dtos;
using TemplateService.Application.User.DTOs;
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

    /// <summary>
    /// Получение информации о пользователе по guid
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserDto>> GetUser(Guid id)
    {
        var user = await _mediator.Send(new GetUserQuery(id));
        return user != null ? Ok(user) : NotFound();
    }

    /// <summary>
    /// Поиск пользователя
    /// </summary>
    [HttpPost("search")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<UserDto>>> SearchUser([FromBody] SearchUserQuery query)
    {
        var user = await _mediator.Send(query);
        return user.Count != 0 ? Ok(user) : NoContent();
    }

    /// <summary>
    /// Создание пользователя
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(CreatedUserResult), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [AllowAnonymous] // TODO Убрать это, после фазы тестирования и добавить проверку на создание только администратором
    public async Task<ActionResult<CreatedUserResult>> CreateUser([FromBody] CreateUserCommand command)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var createdUser = await _mediator.Send(command);

        // Возвращаем 201 Created с Location на нового пользователя
        return CreatedAtAction(nameof(GetUser), new { id = createdUser.UserId }, createdUser);
    }

    /// <summary>
    /// Удаление пользователя (только администратор)
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        await _mediator.Send(new DeleteUserQuery(id));
        return NoContent(); // 204
    }

    /// <summary>
    /// Изменение настроек своего профиля
    /// </summary>
    [HttpPatch("me/profile")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<UserDto>> UpdateUserProfile([FromBody] UpdateUserProfileCommand profileCommand)
    {
        var updatedUser = await _mediator.Send(profileCommand);
        return Ok(updatedUser);
    }
    
    /// <summary>
    /// Изменение настроек получения уведомлений
    /// </summary>
    [HttpPatch("me/notification_preferences")]
    [ProducesResponseType(typeof(UserNotificationPreferencesDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<UserNotificationPreferencesDto>> UpdateUserNotificationPreferences([FromBody] UpdateNotificationPreferencesCommand command)
    {
        var updatedUser = await _mediator.Send(command);
        return Ok(updatedUser);
    }
    
    /// <summary>
    /// Получение настроек получения уведомлений
    /// </summary>
    [HttpGet("me/notification_preferences")]
    [ProducesResponseType(typeof(UserNotificationPreferencesDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<UserNotificationPreferencesDto>> GetUserNotificationPreferences(GetNotificationPreferencesQuery command)
    {
        var userNotificationPreferences = await _mediator.Send(command);
        return Ok(userNotificationPreferences);
    }

    /// <summary>
    /// Изменение своего пароля
    /// </summary>
    [HttpPatch("me/password")]
    [ProducesResponseType(typeof(Unit), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<Unit>> UpdateUserPassword([FromBody] UpdateUserPasswordCommand profileCommand)
    {
        await _mediator.Send(profileCommand);
        return Ok();
    }

    /// <summary>
    /// Обновление своего аватара
    /// </summary>
    [HttpPatch("me/avatar")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(UpdatedUserAvatarResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<UpdatedUserAvatarResult>> UpdateAvatar([FromForm] UpdateUserAvatarCommand avatar)
    {
        var result = await _mediator.Send(avatar);
        return Ok(result);
    }

    /// <summary>
    /// Удаление своего аватара
    /// </summary>
    [HttpDelete("me/avatar")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> DeleteAvatar()
    {
        var result = await _mediator.Send(new DeleteUserAvatarCommand()); // ← если у тебя есть такой Command
        return Ok(result);
    }

    /// <summary>
    /// Получение аватара по ID
    /// </summary>
    [HttpGet("{userId:guid}/avatar")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAvatar([FromRoute] Guid userId)
    {
        var result = await _mediator.Send(new GetUserAvatarQuery(userId));

        if (result == null || result.AvatarBytes == null || result.AvatarBytes.Length == 0)
        {
            // Аватар не найден — возвращаем 404
            return NotFound();
        }
        return File(result.AvatarBytes, result.MimeType);
    }
}