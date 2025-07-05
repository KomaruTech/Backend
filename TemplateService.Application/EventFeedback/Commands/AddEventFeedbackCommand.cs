using TemplateService.Application.EventFeedback.DTOs;

namespace TemplateService.Application.EventFeedback.Commands;

public record AddEventFeedbackCommand(
    Guid EventId,
    short Rating,
    string? Comment
    ) : IRequest<EventFeedbackDto>;