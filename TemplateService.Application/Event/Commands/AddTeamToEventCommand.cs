using TemplateService.Application.Event.DTOs;

namespace TemplateService.Application.Event.Commands;

public record AddTeamToEventCommand
(
    Guid EventId,
    Guid TeamId
) : IRequest<EventDto>;