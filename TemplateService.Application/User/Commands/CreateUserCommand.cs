using TemplateService.Application.User.DTOs;
using TemplateService.Domain.Enums;

namespace TemplateService.Application.User.Commands;

public record CreateUserCommand(
    string Name,
    string Surname,
    string Email,
    UserRoleEnum Role
) : IRequest<CreatedUserResult>; // Вернём созданного пользователя