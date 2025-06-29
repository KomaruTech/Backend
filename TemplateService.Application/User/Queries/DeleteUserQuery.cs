namespace TemplateService.Application.User.Queries;

public record DeleteUserQuery(string Login) : IRequest<Unit>;