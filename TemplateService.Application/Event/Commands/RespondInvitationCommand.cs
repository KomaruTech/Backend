namespace TemplateService.Application.Event.Commands;

public record RespondInvitationCommand(
    Guid EventId,
    RespondInvitationStatus Status
) : IRequest<Unit>;

public enum RespondInvitationStatus
{
    approved,
    rejected
}