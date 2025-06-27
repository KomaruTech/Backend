using MediatR;
using TemplateService.Application.User.DTOs;
using TemplateService.Domain.Enums;

namespace TemplateService.Application.User.Commands;

public record CreateUserCommand(
    string Name,
    string Surname,
    string Password,
    string Email,
    UserRoleEnum Role
) : IRequest<UserDto>; // Вернём созданного пользователя