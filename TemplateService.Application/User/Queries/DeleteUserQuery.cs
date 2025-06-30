namespace TemplateService.Application.User.Queries;

public record DeleteUserQuery(Guid Id) : IRequest<Unit>;