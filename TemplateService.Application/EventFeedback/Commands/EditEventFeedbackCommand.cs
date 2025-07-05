using TemplateService.Application.EventFeedback.DTOs;

namespace TemplateService.Application.EventFeedback.Commands;

public record EditEventFeedbackCommand(
    Guid EventId,
    short Rating,
    string? Comment
) : IRequest<EventFeedbackDto>;