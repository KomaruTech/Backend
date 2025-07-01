namespace TemplateService.Application.Auth.Commands;

public record LoginUserCommand(string Login, string Password) : IRequest<LoginUserResult>;