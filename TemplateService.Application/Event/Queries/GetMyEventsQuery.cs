using TemplateService.Application.Event.DTOs;

namespace TemplateService.Application.Event.Queries;

public record GetMyEventsQuery() : IRequest<List<EventDto>>;