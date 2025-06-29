using Microsoft.EntityFrameworkCore;
using TemplateService.Application.Auth.Services;
using TemplateService.Application.User.Services;
using TemplateService.Infrastructure.Persistence;

namespace TemplateService.Application.User.Queries;

internal class DeleteUserQueryHandler : IRequestHandler<DeleteUserQuery, Unit>
{
    private readonly TemplateDbContext _dbContext;
    private readonly IUserValidationService _userValidationService;
    private readonly ICurrentUserService _currentUserService;

    public DeleteUserQueryHandler(
        TemplateDbContext dbContext,
        IUserValidationService userValidationService,
        ICurrentUserService currentUserService
    )
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
        _userValidationService = userValidationService;
    }


    public async Task<Unit> Handle(DeleteUserQuery query, CancellationToken ct)
    {
        var userRole = _currentUserService.GetUserRole();
        _userValidationService.ValidateDeletePermissions(userRole);
        
        var user = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Login == query.Login, ct);
        
        if (user == null)
            throw new InvalidOperationException($"User with Login {query.Login} not found.");
        
        _dbContext.Users.Remove(user);
        await _dbContext.SaveChangesAsync(ct);

        return Unit.Value;
    }
}