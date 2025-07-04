using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TemplateService.Application.Auth.Services;
using TemplateService.Application.Teams.Dtos;
using TemplateService.Application.Teams.Services;
using TemplateService.Infrastructure.Persistence;

namespace TemplateService.Application.Teams.Commands;

internal class DeleteTeamMemberCommandHandler : IRequestHandler<DeleteTeamMemberCommand, TeamsDto>
{
    private readonly TemplateDbContext _dbContext;
    private readonly ITeamValidationService _teamValidationService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;

    public DeleteTeamMemberCommandHandler(
        TemplateDbContext dbContext,
        ITeamValidationService teamValidationService,
        ICurrentUserService currentUserService,
        IMapper mapper)
    {
        _dbContext = dbContext;
        _teamValidationService = teamValidationService;
        _currentUserService = currentUserService;
        _mapper = mapper;
    }

    public async Task<TeamsDto> Handle(DeleteTeamMemberCommand request, CancellationToken ct)
    {
        var currentUserId = _currentUserService.GetUserId();
        var currentUserRole = _currentUserService.GetUserRole();

        var team = await _dbContext.Teams
                       .Include(t => t.Users)
                       .ThenInclude(ut => ut.User)
                       .FirstOrDefaultAsync(t => t.Id == request.TeamId, ct)
                   ?? throw new InvalidOperationException($"Team with id {request.TeamId} not found.");

        var memberToDelete = team.Users
                                 .FirstOrDefault(m => m.UserId == request.UserId)
                             ?? throw new InvalidOperationException($"User with id {request.UserId} is not a member of team {request.TeamId}.");


        _teamValidationService.ValidateDeleteMemberPermission(
            userThatDeletesId: currentUserId,
            userToDeleteId: request.UserId,
            teamOwnerId: team.OwnerId,
            userThatDeletesRoles: currentUserRole
        );

        team.Users.Remove(memberToDelete);
        await _dbContext.SaveChangesAsync(ct);

        return _mapper.Map<TeamsDto>(team);
    }
}