using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TemplateService.Application.Auth.Services;
using TemplateService.Application.Event.DTOs;
using TemplateService.Application.Event.Services;
using TemplateService.Domain.Entities;
using TemplateService.Domain.Enums;
using TemplateService.Infrastructure.Persistence;

namespace TemplateService.Application.Event.Commands;

internal class RemoveUserFromEventCommandHandler : IRequestHandler<RemoveUserFromEventCommand, EventDto>
{
    private readonly TemplateDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;
    private readonly IEventValidationService _eventValidationService;
    private readonly IMapper _mapper;

    public RemoveUserFromEventCommandHandler(
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

    public async Task<EventDto> Handle(RemoveUserFromEventCommand command, CancellationToken cancellationToken)
    {
        var eventEntity = await _dbContext.Events.FindAsync([command.EventId], cancellationToken);
        if (eventEntity == null)
            throw new InvalidOperationException($"Event with ID {command.EventId} not found.");
        
        var userEntity = await _dbContext.Users.FindAsync([command.UserId], cancellationToken);
        if (userEntity == null)
            throw new InvalidOperationException($"User with ID {command.UserId} not found.");
        
        var userId = _currentUserService.GetUserId();
        var userRole = _currentUserService.GetUserRole();

        _eventValidationService.ValidateInvitePermissions(userId, eventEntity.CreatedById, userRole);

        // Проверяем, существует ли уже приглашение/участие для command.UserId на это мероприятие
        var existingParticipant = await _dbContext.EventParticipants
            .FirstOrDefaultAsync(ep => ep.EventId == command.EventId && ep.UserId == command.UserId, cancellationToken);

        if (existingParticipant == null)
        {
            throw new InvalidOperationException($"User with ID {command.UserId} is not invited or participating in the event.");
        }
        
        _dbContext.EventParticipants.Remove(existingParticipant);
        await _dbContext.SaveChangesAsync(cancellationToken);
        // Актуальные данные
        var updatedEvent = await _dbContext.Events
            .Include(e => e.Participants)
            .Include(e => e.EventTeams)
            .FirstOrDefaultAsync(e => e.Id == command.EventId, cancellationToken);

        return _mapper.Map<EventDto>(updatedEvent);
    }
}