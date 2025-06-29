using TemplateService.Application.PasswordService;

namespace TemplateService.Application.User.Commands;

using AutoMapper;
using TemplateService.Application.Auth.Services;
using DTOs;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Services;

internal class UpdateUserPasswordCommandHandler : IRequestHandler<UpdateUserPasswordCommand, Unit>
{
    private readonly TemplateDbContext _dbContext;
    private readonly IUserValidationService _userValidationService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IPasswordHelper _passwordHelper;

    public UpdateUserPasswordCommandHandler(
        TemplateDbContext dbContext,
        IUserValidationService userValidationService,
        IPasswordHelper passwordHelper,
        ICurrentUserService currentUserService
    )
    {
        _dbContext = dbContext;
        _userValidationService = userValidationService;
        _currentUserService = currentUserService;
        _passwordHelper = passwordHelper;
    }

    public async Task<Unit> Handle(UpdateUserPasswordCommand command, CancellationToken ct)
    {
        // Валидация нового пароля
        _userValidationService.ValidatePassword(command.newPassword);

        var userId = _currentUserService.GetUserId();
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId, ct);

        if (user == null)
            throw new ArgumentException($"User with ID {userId} not found");

        // Проверка старого пароля
        if (!_passwordHelper.VerifyPassword(user.PasswordHash, command.oldPassword))
            throw new UnauthorizedAccessException("Password is incorrect");

        // Проверка, что новый пароль не совпадает со старым (по хэшу)
        var newPasswordHash = _passwordHelper.HashPassword(command.newPassword);
        if (newPasswordHash == user.PasswordHash)
            throw new ArgumentException("New password must be different from the old password");
        
        user.PasswordHash = newPasswordHash;
        
        await _dbContext.SaveChangesAsync(ct);

        return Unit.Value;
    }
}