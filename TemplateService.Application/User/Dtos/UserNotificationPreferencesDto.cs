namespace TemplateService.Application.User.Dtos;


public class UserNotificationPreferencesDto
{
    public Guid Id { get; set; }
    public bool NotifyTelegram { get; set; }
    public bool NotifyBeforeOneDay { get; set; }
    public bool NotifyBeforeOneHour { get; set; }
}