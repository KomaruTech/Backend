using MediatR;
using TemplateService.Application.User.DTOs;

namespace TemplateService.Application.User.Commands;

public record CreateUserCommand(
    string Login,
    string Name,
    string Password,
    string Surname,
    string Email
) : IRequest<UserDto>; // Вернём созданного пользователя