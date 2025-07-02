using Microsoft.EntityFrameworkCore;
using TemplateService.Application.Auth.Services;
using TemplateService.Application.Event.Services;
using TemplateService.Domain.Entities;
using TemplateService.Domain.Enums;
using TemplateService.Infrastructure.Persistence;

namespace TemplateService.Application.Event.Commands;

internal class InviteUserToEventCommandHandler : IRequestHandler<InviteUserToEventCommand, Unit>
{
    private readonly TemplateDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;
    private readonly IEventValidationService _eventValidationService;

    public InviteUserToEventCommandHandler(
        TemplateDbContext dbContext,
        ICurrentUserService currentUserService,
        IEventValidationService eventValidationService
    )
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
        _eventValidationService = eventValidationService;
    }

    public async Task<Unit> Handle(InviteUserToEventCommand command, CancellationToken cancellationToken)
    {
        var eventEntity = await _dbContext.Events.FindAsync([command.EventId], cancellationToken);
        if (eventEntity == null)
            throw new InvalidOperationException($"Event with ID {command.EventId} not found.");

        var userId = _currentUserService.GetUserId();
        var userRole = _currentUserService.GetUserRole();

        _eventValidationService.ValidateInvitePermissions(userId, eventEntity.CreatedById, userRole);

        // Проверяем, существует ли уже приглашение/участие для command.UserId на это мероприятие
        var existingParticipant = await _dbContext.EventParticipants
            .FirstOrDefaultAsync(ep => ep.EventId == command.EventId && ep.UserId == command.UserId, cancellationToken);

        if (existingParticipant != null)
        {
            throw new InvalidOperationException($"User with ID {command.UserId} is already invited or participating in the event.");
        }

        // Создаём новую запись участия со статусом приглашения pending
        var newParticipant = new EventParticipantEntity
        {
            EventId = command.EventId,
            UserId = command.UserId,
            IsSpeaker = command.asSpeaker,
            AttendanceResponse = AttendanceResponseEnum.pending
        };

        _dbContext.EventParticipants.Add(newParticipant);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}