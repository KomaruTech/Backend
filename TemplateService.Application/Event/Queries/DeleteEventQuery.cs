namespace TemplateService.Application.Event.Queries;

public record DeleteEventQuery(Guid Id) : IRequest<Unit>;