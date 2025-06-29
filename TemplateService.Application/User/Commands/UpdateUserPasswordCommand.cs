namespace TemplateService.Application.User.Commands;

public record UpdateUserPasswordCommand(
    string oldPassword,
    string newPassword
): IRequest<Unit>; // Вернём созданного пользователя;