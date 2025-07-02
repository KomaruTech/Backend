using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TemplateService.Application.Auth.Services;
using TemplateService.Application.Event.DTOs;
using TemplateService.Application.Event.Services;
using TemplateService.Domain.Entities;
using TemplateService.Domain.Enums;
using TemplateService.Infrastructure.Persistence;

namespace TemplateService.Application.Event.Commands;

internal class AddTeamToEventCommandHandler : IRequestHandler<AddTeamToEventCommand, EventDto>
{
    private readonly TemplateDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;
    private readonly IEventValidationService _eventValidationService;
    private readonly IMapper _mapper;

    public AddTeamToEventCommandHandler(
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

    public async Task<EventDto> Handle(AddTeamToEventCommand command, CancellationToken cancellationToken)
    {
        var eventEntity = await _dbContext.Events.FindAsync([command.EventId], cancellationToken);
        if (eventEntity == null)
            throw new InvalidOperationException($"Event with ID {command.EventId} not found.");

        if (eventEntity.Type != EventTypeEnum.group)
            throw new InvalidOperationException($"Event with ID {command.EventId} is not a group event, so it is not possible to add teams to it.");

        var teamEntity = await _dbContext.Teams.FindAsync([command.TeamId], cancellationToken);
        if (teamEntity == null)
            throw new InvalidOperationException($"Team with ID {command.TeamId} not found.");

        var userId = _currentUserService.GetUserId();
        var userRole = _currentUserService.GetUserRole();

        _eventValidationService.ValidateInvitePermissions(userId, eventEntity.CreatedById, userRole);

        // Проверяем, существует ли уже приглашение/участие для command.TeamId на это мероприятие
        var existingTeamParticipant = await _dbContext.EventTeams
            .FirstOrDefaultAsync(ep => ep.EventId == command.EventId && ep.TeamId == command.TeamId, cancellationToken);

        if (existingTeamParticipant != null)
            throw new InvalidOperationException($"User with ID {command.TeamId} is already invited or participating in the event.");

        // Создаём новую запись участия со статусом приглашения pending
        var newParticipant = new EventTeamsEntity
        {
            EventId = command.EventId,
            TeamId = command.TeamId
        };

        _dbContext.EventTeams.Add(newParticipant);
        await _dbContext.SaveChangesAsync(cancellationToken);
        
        // Актуальные данные
        var updatedEvent = await _dbContext.Events
            .Include(e => e.Participants)
            .Include(e => e.EventTeams)
            .FirstOrDefaultAsync(e => e.Id == command.EventId, cancellationToken);

        return _mapper.Map<EventDto>(updatedEvent);
    }
}