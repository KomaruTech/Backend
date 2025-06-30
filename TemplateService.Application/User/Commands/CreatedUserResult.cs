namespace TemplateService.Application.User.Commands;

public record CreatedUserResult(
    Guid UserId,
    string Login,
    string Password
);