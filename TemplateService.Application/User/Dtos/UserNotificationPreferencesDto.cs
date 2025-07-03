namespace TemplateService.Application.User.Dtos;


public record UserNotificationPreferencesDto
{
    public Guid Id { get; init; }
    public bool NotifyTelegram { get; init; }
    public bool NotifyBeforeOneDay { get; init; }
    public bool NotifyBeforeOneHour { get; init; }
}