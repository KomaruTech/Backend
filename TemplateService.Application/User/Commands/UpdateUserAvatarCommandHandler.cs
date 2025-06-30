using Microsoft.EntityFrameworkCore;
using TemplateService.Application.Auth.Services;
using TemplateService.Application.User.Services;
using TemplateService.Infrastructure.Persistence;

namespace TemplateService.Application.User.Commands;

internal class UpdateUserAvatarCommandHandler : IRequestHandler<UpdateUserAvatarCommand, UpdatedUserAvatarResult>
{
    private readonly TemplateDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUserValidationService _userValidationService;
    private readonly IUserHelperService _userHelperService;

    public UpdateUserAvatarCommandHandler(
        TemplateDbContext dbContext,
        ICurrentUserService currentUserService,
        IUserValidationService userValidationService,
        IUserHelperService userHelperService
    )
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
        _userValidationService = userValidationService;
        _userHelperService = userHelperService;
    }

    public async Task<UpdatedUserAvatarResult> Handle(UpdateUserAvatarCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.GetUserId();

        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken)
                   ?? throw new InvalidOperationException($"User with id {userId} not found.");
        
        var file = request.Avatar;
        _userValidationService.ValidateAvatar(file);

        using var ms = new MemoryStream();
        await file.CopyToAsync(ms, cancellationToken);
        user.Avatar = ms.ToArray();
        user.AvatarMimeType = file.ContentType;

        await _dbContext.SaveChangesAsync(cancellationToken);
        
        return new UpdatedUserAvatarResult(_userHelperService.GetAvatarUrl(userId));
    }
}