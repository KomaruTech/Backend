namespace TemplateService.Application.User.Commands;

public record UpdateUserPasswordCommand(
    string OldPassword,
    string NewPassword
): IRequest<Unit>; // Вернём созданного пользователя;