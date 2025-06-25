namespace TemplateService.Application.User.DTOs;

public record UserDto(
    Guid Id,
    string Login,
    string Name,
    string PasswordHash,
    string Surname,
    string? Email,
    string? TelegramId,
    Guid NotificationPreferencesId,
    byte[]? Avatar);