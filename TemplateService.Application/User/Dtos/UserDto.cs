using TemplateService.Domain.Enums;

namespace TemplateService.Application.User.DTOs;

public record UserDto(
    Guid Id,
    string Login,
    string Name,
    string Surname,
    UserRoleEnum Role,
    string Email,
    string? TelegramId
    );