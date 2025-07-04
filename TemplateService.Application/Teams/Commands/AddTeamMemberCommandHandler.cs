using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TemplateService.Application.Auth.Services;
using TemplateService.Application.Teams.Commands;
using TemplateService.Application.Teams.Dtos;
using TemplateService.Application.Teams.Services;
using TemplateService.Application.User.DTOs;
using TemplateService.Domain.Entities;
using TemplateService.Infrastructure.Persistence;

namespace TemplateService.Application.Teams.Commands;

internal class AddTeamMemberCommandHandler : IRequestHandler<AddTeamMemberCommand, TeamsDto>
{
    private readonly TemplateDbContext _dbContext;
    private readonly ITeamValidationService _teamValidationService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;

    public AddTeamMemberCommandHandler(
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

    public async Task<TeamsDto> Handle(AddTeamMemberCommand request, CancellationToken ct)
    {
        var userId = _currentUserService.GetUserId();
        var userRole = _currentUserService.GetUserRole();

        var team = await GetTeamAsync(request.TeamId, ct); // просто проверка на существование
        var userToAdd = await GetUserAsync(request.UserId, ct);

        if (await IsUserAlreadyMemberAsync(request.TeamId, request.UserId, ct))
            throw new InvalidOperationException("User is already in a team");
    
        _teamValidationService.ValidateAddToTeamPermission(userId, userToAdd.Id, userRole);

        await AddUserToTeamAsync(request.TeamId, request.UserId, ct);
        
        var updatedTeam = await _dbContext.Teams
            .Include(t => t.Users)
            .ThenInclude(ut => ut.User)
            .FirstOrDefaultAsync(t => t.Id == request.TeamId, ct);

        return _mapper.Map<TeamsDto>(updatedTeam!);
    }

    private async Task<TeamsEntity> GetTeamAsync(Guid teamId, CancellationToken ct)
    {
        return await _dbContext.Teams
            .FirstOrDefaultAsync(t => t.Id == teamId, ct)
            ?? throw new InvalidOperationException($"Team with id {teamId} not found.");
    }

    private async Task<UserEntity> GetUserAsync(Guid userId, CancellationToken ct)
    {
        return await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Id == userId, ct)
            ?? throw new InvalidOperationException($"User with id {userId} not found.");
    }

    private async Task<bool> IsUserAlreadyMemberAsync(Guid teamId, Guid userId, CancellationToken ct)
    {
        return await _dbContext.UserTeams
            .AnyAsync(ut => ut.TeamId == teamId && ut.UserId == userId, ct);
    }

    private async Task AddUserToTeamAsync(Guid teamId, Guid userId, CancellationToken ct)
    {
        var userTeam = new UserTeamsEntity
        {
            TeamId = teamId,
            UserId = userId
        };

        await _dbContext.UserTeams.AddAsync(userTeam, ct);
        await _dbContext.SaveChangesAsync(ct);
    }
}