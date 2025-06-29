using Microsoft.EntityFrameworkCore;
using TemplateService.Infrastructure.Persistence;

namespace TemplateService.Application.User.Queries;

internal class GetUserAvatarQueryHandler : IRequestHandler<GetUserAvatarQuery, UserAvatarResult>
{
    private readonly TemplateDbContext _dbContext;

    public GetUserAvatarQueryHandler(TemplateDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<UserAvatarResult> Handle(GetUserAvatarQuery query, CancellationToken cancellationToken)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == query.UserId, cancellationToken)
                   ?? throw new InvalidOperationException($"User with id {query.UserId} found.");

        if (user.Avatar == null || string.IsNullOrWhiteSpace(user.AvatarMimeType))
            throw new FileNotFoundException("Avatar not found.");

        return new UserAvatarResult(user.Avatar, user.AvatarMimeType);
    }
}