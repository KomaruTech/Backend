namespace TemplateService.Application.User.DTOs;

public record UserDto(
    int Id,
    string Login,
    string Name,
    string PasswordHash,
    string Surname,
    string? Email,
    string? TelegramId,
    int NotificationPreferencesId);