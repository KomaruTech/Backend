using TemplateService.Application.Event.DTOs;

namespace TemplateService.Application.Event.Queries;

public record SearchInIntervalQuery(DateTime? StartDate, DateTime? EndDate) : IRequest<List<EventDto>>;