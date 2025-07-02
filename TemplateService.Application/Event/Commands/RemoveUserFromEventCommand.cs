using TemplateService.Application.Event.DTOs;

namespace TemplateService.Application.Event.Commands;

public record RemoveUserFromEventCommand
(
    Guid EventId,
    Guid UserId
) : IRequest<EventDto>;