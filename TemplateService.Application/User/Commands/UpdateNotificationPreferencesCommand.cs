using TemplateService.Application.User.Dtos;

namespace TemplateService.Application.User.Commands;

public record UpdateNotificationPreferencesCommand(
    bool? NotifyTelegram,
    bool? NotifyBeforeOneDay,
    bool? NotifyBeforeOneHour
) : IRequest<UserNotificationPreferencesDto>;