using TemplateService.Application.Event.DTOs;

namespace TemplateService.Application.Event.Queries;

public record GetSuggestedEventsQuery() : IRequest<List<EventDto>>;