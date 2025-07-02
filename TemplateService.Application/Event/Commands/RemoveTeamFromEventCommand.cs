using TemplateService.Application.Event.DTOs;

namespace TemplateService.Application.Event.Commands;

public record RemoveTeamFromEventCommand
(
    Guid EventId,
    Guid TeamId
) : IRequest<EventDto>;