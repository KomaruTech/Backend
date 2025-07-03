namespace TemplateService.Application.User.Dtos;


public record UserNotificationPreferencesDto(
     Guid Id,
     bool NotifyTelegram,
     bool NotifyBeforeOneDay,
     bool NotifyBeforeOneHour);