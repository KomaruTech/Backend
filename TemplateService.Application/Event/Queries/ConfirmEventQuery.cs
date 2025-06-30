namespace TemplateService.Application.Event.Queries;

public record ConfirmEventQuery(Guid Id) : IRequest<Unit>;