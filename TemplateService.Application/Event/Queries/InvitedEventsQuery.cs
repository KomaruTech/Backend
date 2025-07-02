using TemplateService.Application.Event.DTOs;

namespace TemplateService.Application.Event.Queries;

public record InvitedEventsQuery() : IRequest<List<EventDto>>;