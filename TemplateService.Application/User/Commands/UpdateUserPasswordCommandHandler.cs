using Microsoft.EntityFrameworkCore;
using TemplateService.Application.Auth.Services;
using TemplateService.Application.PasswordService;
using TemplateService.Application.User.Services;
using TemplateService.Infrastructure.Persistence;

namespace TemplateService.Application.User.Commands;

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

    public async Task<Unit> Handle(UpdateUserPasswordCommand command, CancellationToken cancellationToken)
    {
        // Валидация нового пароля
        _userValidationService.ValidatePassword(command.newPassword);

        var userId = _currentUserService.GetUserId();
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken)
                   ?? throw new InvalidOperationException($"User with id {userId} found.");

        // Проверка старого пароля
        if (!_passwordHelper.VerifyPassword(user.PasswordHash, command.oldPassword))
            throw new UnauthorizedAccessException("Password is incorrect");

        // Проверка, что новый пароль не совпадает со старым (по хэшу)
        var newPasswordHash = _passwordHelper.HashPassword(command.newPassword);
        if (newPasswordHash == user.PasswordHash)
            throw new ArgumentException("New password must be different from the old password");
        
        user.PasswordHash = newPasswordHash;
        
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}