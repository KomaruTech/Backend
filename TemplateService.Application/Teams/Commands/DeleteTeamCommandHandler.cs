using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TemplateService.Application.Auth.Services;
using TemplateService.Application.Teams.Services;
using TemplateService.Infrastructure.Persistence;

namespace TemplateService.Application.Teams.Commands;

internal class DeleteTeamCommandHandler : IRequestHandler<DeleteTeamCommand, Unit>
{
    private readonly TemplateDbContext _dbContext;
    private readonly ITeamValidationService _teamValidationService;
    private readonly ICurrentUserService _currentUserService;

    public DeleteTeamCommandHandler(
        TemplateDbContext dbContext,
        ITeamValidationService teamValidationService,
        ICurrentUserService currentUserService
    )
    {
        _dbContext = dbContext;
        _teamValidationService = teamValidationService;
        _currentUserService = currentUserService;
    }

    public async Task<Unit> Handle(DeleteTeamCommand request, CancellationToken ct)
    {
        var userId = _currentUserService.GetUserId();
        var userRole = _currentUserService.GetUserRole();

        var team = await _dbContext.Teams
                       .FirstOrDefaultAsync(t => t.Id == request.TeamId, ct)
                   ?? throw new InvalidOperationException($"Team with id {request.TeamId} not found.");

        _teamValidationService.ValidateDeletePermission(userId, team.OwnerId, userRole);

        _dbContext.Teams.Remove(team);
        await _dbContext.SaveChangesAsync(ct);

        return Unit.Value;
    }
}