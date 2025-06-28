using MediatR;
using Microsoft.EntityFrameworkCore;
using TemplateService.Application.PasswordService;
using TemplateService.Application.TokenService;
using TemplateService.Application.User.DTOs;
using TemplateService.Infrastructure.Persistence;

namespace TemplateService.Application.Auth.Commands;

public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, LoginUserResult?>
{
    private readonly TemplateDbContext _dbContext;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;

    public LoginUserCommandHandler
    (
        TemplateDbContext dbContext,
        IPasswordHasher passwordHasher,
        ITokenService tokenService
    )
    {
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
    }

    public async Task<LoginUserResult?> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        var userEntity = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Login == request.Login, cancellationToken);

        if (userEntity == null || !_passwordHasher.VerifyPassword(userEntity.PasswordHash, request.Password))
        {
            return null;
        }

        var userDto = new UserDto(
            Id: userEntity.Id,
            Login: userEntity.Login,
            Name: userEntity.Name,
            Surname: userEntity.Surname,
            Role: userEntity.Role,
            Email: userEntity.Email,
            TelegramId: userEntity.TelegramId,
            NotificationPreferencesId: userEntity.NotificationPreferencesId,
            Avatar: userEntity.Avatar);

        var token = _tokenService.CreateToken(userDto);

        return new LoginUserResult(userDto, token);
    }
}