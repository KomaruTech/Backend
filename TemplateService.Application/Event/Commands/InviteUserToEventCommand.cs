namespace TemplateService.Application.Event.Commands;

public record InviteUserToEventCommand
(
    Guid EventId,
    Guid UserId,
    bool asSpeaker = false
) : IRequest<Unit>;