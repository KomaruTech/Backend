using Microsoft.EntityFrameworkCore;
using TemplateService.Application.Auth.Services;
using TemplateService.Application.User.Services;
using TemplateService.Infrastructure.Persistence;

namespace TemplateService.Application.User.Commands;

internal class DeleteUserAvatarCommandHandler : IRequestHandler<DeleteUserAvatarCommand, Unit>
{
    private readonly TemplateDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;

    public DeleteUserAvatarCommandHandler(
        TemplateDbContext dbContext,
        ICurrentUserService currentUserService,
        IUserHelperService userHelperService)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
    }

    public async Task<Unit> Handle(DeleteUserAvatarCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.GetUserId();

        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken)
                   ?? throw new InvalidOperationException($"User with id {userId} not found.");

        user.Avatar = null;
        user.AvatarMimeType = null;

        await _dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value; // нет аватара => null
    }
}