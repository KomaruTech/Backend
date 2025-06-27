namespace TemplateService.Application.User.DTOs;

/// <summary>
/// Модель данных для входа пользователя
/// </summary>
public record UserLoginDto(
    string Username,
    string Password
);