namespace TemplateService.Application.EventFeedback.Commands;

public record DeleteEventFeedbackCommand(
    Guid EventId
) : IRequest<Unit>;