using Microsoft.EntityFrameworkCore;
using TemplateService.Application.Auth.Services;
using TemplateService.Application.Event.Services;
using TemplateService.Domain.Entities;
using TemplateService.Domain.Enums;
using TemplateService.Infrastructure.Persistence;

namespace TemplateService.Application.Event.Commands;

internal class RespondInvitationCommandHandler : IRequestHandler<RespondInvitationCommand, Unit>
{
    private readonly TemplateDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;
    private readonly IEventValidationService _eventValidationService;

    public RespondInvitationCommandHandler(
        TemplateDbContext dbContext,
        ICurrentUserService currentUserService,
        IEventValidationService eventValidationService
    )
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
        _eventValidationService = eventValidationService;
    }

    public async Task<Unit> Handle(RespondInvitationCommand command, CancellationToken cancellationToken)
    {
        var eventEntity = await _dbContext.Events.FindAsync([command.EventId], cancellationToken);
        if (eventEntity == null)
            throw new InvalidOperationException($"Event with ID {command.EventId} not found.");

        var userId = _currentUserService.GetUserId();

        // Проверяем, есть ли приглашение/участие для текущего пользователя на этот ивент
        var participant = await _dbContext.EventParticipants
            .FirstOrDefaultAsync(ep => ep.EventId == command.EventId && ep.UserId == userId, cancellationToken);

        if (participant == null)
            throw new InvalidOperationException("No invitation or participation found for this user on the event.");

        if (participant.AttendanceResponse != AttendanceResponseEnum.pending)
            throw new InvalidOperationException("Invitation response has already been processed.");

        // Обновляем статус в зависимости от команды
        participant.AttendanceResponse = command.Status switch
        {
            RespondInvitationStatus.approved => AttendanceResponseEnum.approved,
            RespondInvitationStatus.rejected => AttendanceResponseEnum.rejected,
            _ => throw new InvalidOperationException("Invalid invitation response status.")
        };

        await _dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}