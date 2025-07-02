using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TemplateService.Application.Auth.Services;
using TemplateService.Application.Event.DTOs;
using TemplateService.Application.Event.Services;
using TemplateService.Domain.Entities;
using TemplateService.Infrastructure.Persistence;

namespace TemplateService.Application.Event.Commands;

internal class RemoveTeamFromEventCommandHandler : IRequestHandler<RemoveTeamFromEventCommand, EventDto>
{
    private readonly TemplateDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;
    private readonly IEventValidationService _eventValidationService;
    private readonly IMapper _mapper;

    public RemoveTeamFromEventCommandHandler(
        TemplateDbContext dbContext,
        ICurrentUserService currentUserService,
        IEventValidationService eventValidationService,
        IMapper mapper
    )
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
        _eventValidationService = eventValidationService;
        _mapper = mapper;
    }

    public async Task<EventDto> Handle(RemoveTeamFromEventCommand command, CancellationToken cancellationToken)
    {
        var eventEntity = await _dbContext.Events.FindAsync([command.EventId], cancellationToken);
        if (eventEntity == null)
            throw new InvalidOperationException($"Event with ID {command.EventId} not found.");
        
        var teamEntity = await _dbContext.Teams.FindAsync([command.TeamId], cancellationToken);
        if (teamEntity == null)
            throw new InvalidOperationException($"Team with ID {command.TeamId} not found.");
        
        var userId = _currentUserService.GetUserId();
        var userRole = _currentUserService.GetUserRole();

        _eventValidationService.ValidateInvitePermissions(userId, eventEntity.CreatedById, userRole);

        // Проверяем, добавлена ли команда к мероприятию
        var existingTeam = await _dbContext.EventTeams
            .FirstOrDefaultAsync(et => et.EventId == command.EventId && et.TeamId == command.TeamId, cancellationToken);

        if (existingTeam == null)
        {
            throw new InvalidOperationException($"Team with ID {command.TeamId} is not added to the event.");
        }

        _dbContext.EventTeams.Remove(existingTeam);
        await _dbContext.SaveChangesAsync(cancellationToken);

        // Актуальные данные
        var updatedEvent = await _dbContext.Events
            .Include(e => e.Participants)
            .Include(e => e.EventTeams)
            .FirstOrDefaultAsync(e => e.Id == command.EventId, cancellationToken);

        return _mapper.Map<EventDto>(updatedEvent!);
    }
}