namespace TemplateService.Application.User.Commands;

public record CreatedUserResult(
    string Login,
    string Password
);