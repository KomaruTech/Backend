using TemplateService.Application.User.DTOs;

namespace TemplateService.Application.User.Commands;

public record UpdateUserProfileCommand(
    string? Name,
    string? Surname,
    string? Email,
    string? TelegramId
): IRequest<UserDto>; // Вернём созданного пользователя;