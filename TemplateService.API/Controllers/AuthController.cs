using System.Net.Mime;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TemplateService.Application.TokenService;
using TemplateService.Application.User.DTOs;
using TemplateService.Infrastructure.Persistence; // <-- не забудь пространство имён
using TemplateService.Application.PasswordService;

namespace TemplateService.API.Controllers;

/// <summary>
/// Авторизация
/// </summary>
[ApiController]
[Produces(MediaTypeNames.Application.Json)]
[Route("api/v1/[controller]")]
[AllowAnonymous]
public class AuthController : ControllerBase
{
    private readonly TokenService _tokenService;
    private readonly TemplateDbContext _dbContext;
    private readonly IPasswordHasher _passwordHasher;

    public AuthController(TokenService tokenService, TemplateDbContext dbContext, IPasswordHasher passwordHasher)
    {
        _tokenService = tokenService;
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult Login([FromBody] UserLoginDto userLogin)
    {
        var userFromDb = GetUserFromDatabase(userLogin.Login, userLogin.Password);
        if (userFromDb == null)
        {
            return Unauthorized();
        }

        var token = _tokenService.CreateToken(userFromDb);

        return Ok(new
        {
            user = userFromDb,
            token
        });
    }

    private UserDto? GetUserFromDatabase(string username, string password)
    {
        var userEntity = _dbContext.Users.FirstOrDefault(u => u.Login == username);
        if (userEntity == null)
            return null;

        if (!_passwordHasher.VerifyPassword(userEntity.PasswordHash, password))
            return null;

        return new UserDto(
            Id: userEntity.Id,
            Login: userEntity.Login,
            Name: userEntity.Name,
            Surname: userEntity.Surname,
            Role: userEntity.Role,
            Email: userEntity.Email,
            TelegramId: userEntity.TelegramId,
            NotificationPreferencesId: userEntity.NotificationPreferencesId,
            Avatar: userEntity.Avatar);
    }
}