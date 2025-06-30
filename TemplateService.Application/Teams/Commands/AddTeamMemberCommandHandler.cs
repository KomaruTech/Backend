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

internal class AddTeamMemberCommandHandler
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
        var team = await GetTeamAsync(request.TeamId, ct);
        var userToAdd = await GetUserAsync(request.UserId, ct);

        if (await IsUserAlreadyMemberAsync(request.TeamId, request.UserId, ct))
            throw new InvalidOperationException("Пользователь уже состоит в команде.");

        await AddUserToTeamAsync(request.TeamId, request.UserId, ct);

        return _mapper.Map<TeamsDto>(team);
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