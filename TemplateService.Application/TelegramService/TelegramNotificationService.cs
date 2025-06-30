using Microsoft.EntityFrameworkCore;
using TemplateService.Domain.Entities;
using TemplateService.Domain.Enums;
using TemplateService.Infrastructure.Persistence.Providers.Postgresql;

namespace TemplateService.Application.TelegramService;

public class TelegramNotificationService : ITelegramNotificationService
{
    private readonly TemplatePostgresqlDbContext _dbContext;
    private readonly ITelegramNotificationSender _notificationSender; // В инфраструктуре сервис отправки HTTP

    public TelegramNotificationService(
        TemplatePostgresqlDbContext dbContext,
        ITelegramNotificationSender notificationSender
    )
    {
        _dbContext = dbContext;
        _notificationSender = notificationSender;
    }

    public async Task SendDailyNotificationAsync(CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        var dayLater = now.AddDays(1);

        // Запрос к БД с джойнами, чтобы получить участников, их TelegramId и уже отправленные уведомления
        var participantsWithEvents = await (
                from e in _dbContext.Events
                where e.TimeStart > now && e.TimeStart <= dayLater
                join p in _dbContext.EventParticipants on e.Id equals p.EventId
                join u in _dbContext.Users on p.UserId equals u.Id
                where u.TelegramId != null && u.TelegramId != 0
                join en in _dbContext.EventNotifications.Where(en => en.NotificationType == NotificationTypeEnum.daily)
                    on new { p.UserId, p.EventId } equals new { en.UserId, en.EventId } into sentNotifications
                from sn in sentNotifications.DefaultIfEmpty() // left join, чтобы получить отсутствие уведомления
                select new
                {
                    Event = e,
                    Participant = p,
                    UserTelegramId = u.TelegramId,
                    NotificationSent = sn != null
                })
            .Where(x => !x.NotificationSent) // фильтр - ещё не отправлено уведомление
            .ToListAsync(cancellationToken);

        if (participantsWithEvents.Count == 0)
            return;

        foreach (var item in participantsWithEvents)
        {
            var sendToTgDto = new SendToTelegramEventDto(
                Name: item.Event.Name,
                Description: item.Event.Description,
                TimeStart: item.Event.TimeStart,
                TimeEnd: item.Event.TimeEnd,
                Type: item.Event.Type,
                Location: item.Event.Location,
                TelegramUserId: item.UserTelegramId.Value
            );

            await _notificationSender.SendEventNotificationToTgService(sendToTgDto, cancellationToken);

            _dbContext.EventNotifications.Add(new EventNotificationsEntity
            {
                Id = Guid.NewGuid(),
                UserId = item.Participant.UserId,
                EventId = item.Participant.EventId,
                NotificationType = NotificationTypeEnum.daily,
            });
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}