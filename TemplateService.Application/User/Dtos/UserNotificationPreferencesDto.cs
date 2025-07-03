namespace TemplateService.Application.User.Dtos;


public class UserNotificationPreferencesDto
{
    public Guid Id { get; set; }
    public bool NotifyTelegram { get; set; }
    public bool NotifyBeforeOneDay { get; set; }
    public bool NotifyBeforeOneHour { get; set; }

    // Добавьте конструктор
    public UserNotificationPreferencesDto(Guid id, bool notifyTelegram, bool notifyBeforeOneDay, bool notifyBeforeOneHour)
    {
        Id = id;
        NotifyTelegram = notifyTelegram;
        NotifyBeforeOneDay = notifyBeforeOneDay;
        NotifyBeforeOneHour = notifyBeforeOneHour;
    }
}