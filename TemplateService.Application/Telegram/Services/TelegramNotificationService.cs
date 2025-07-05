using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TemplateService.Application.TelegramService;
using TemplateService.Domain.Entities;
using TemplateService.Domain.Enums;
using TemplateService.Infrastructure.Persistence.Providers.Postgresql;

namespace TemplateService.Application.Telegram.Services;

public class TelegramNotificationService : ITelegramNotificationService
{
    private readonly TemplatePostgresqlDbContext _dbContext;
    private readonly ITelegramNotificationSender _notificationSender;
    private readonly ILogger<TelegramNotificationService> _logger;

    public TelegramNotificationService(
        TemplatePostgresqlDbContext dbContext,
        ITelegramNotificationSender notificationSender,
        ILogger<TelegramNotificationService> logger)
    {
        _dbContext = dbContext;
        _notificationSender = notificationSender;
        _logger = logger;
    }

    public async Task SendDailyNotificationAsync(CancellationToken cancellationToken)
    {
        await SendNotificationBeforeEventAsync(TimeSpan.FromDays(1), NotificationTypeEnum.daily, cancellationToken);
    }

    public async Task SendHourlyNotificationAsync(CancellationToken cancellationToken)
    {
        await SendNotificationBeforeEventAsync(TimeSpan.FromHours(1), NotificationTypeEnum.hourly, cancellationToken);
    }

    private async Task SendNotificationBeforeEventAsync(
        TimeSpan timeBefore,
        NotificationTypeEnum notificationType,
        CancellationToken cancellationToken)
    {
        try
        {
            var now = DateTime.UtcNow;
            var notificationTime = now.Add(timeBefore);

            var participantsWithEvents = await GetParticipantsForNotificationAsync(now, notificationTime, cancellationToken);

            if (participantsWithEvents.Count == 0)
            {
                _logger.LogInformation("No participants found for {NotificationType} notification", notificationType);
                return;
            }

            await ProcessNotificationsAsync(participantsWithEvents, notificationType, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending {NotificationType} notifications", notificationType);
            throw;
        }
    }

    private async Task<List<ParticipantNotificationInfo>> GetParticipantsForNotificationAsync(
        DateTime now,
        DateTime notificationTime,
        CancellationToken cancellationToken)
    {
        return await (
            from e in _dbContext.Events
            where e.TimeStart > now && e.TimeStart <= notificationTime
            join p in _dbContext.EventParticipants on e.Id equals p.EventId
            join u in _dbContext.Users on p.UserId equals u.Id
            where u.TelegramId != null && u.TelegramId != 0
            select new ParticipantNotificationInfo
            {
                Event = e,
                Participant = p,
                UserTelegramId = u.TelegramId.Value
            })
        .ToListAsync(cancellationToken);
    }

    private async Task ProcessNotificationsAsync(
        List<ParticipantNotificationInfo> participants,
        NotificationTypeEnum notificationType,
        CancellationToken cancellationToken)
    {
        foreach (var item in participants)
        {
            try
            {
                // Проверяем, не было ли уже такого уведомления
                var wasSent = await _dbContext.EventNotifications
                    .AnyAsync(en =>
                        en.UserId == item.Participant.UserId &&
                        en.EventId == item.Participant.EventId &&
                        en.NotificationType == notificationType,
                        cancellationToken);

                if (wasSent)
                {
                    _logger.LogDebug("Notification {Type} already sent for user {UserId} and event {EventId}",
                        notificationType, item.Participant.UserId, item.Event.Id);
                    continue;
                }

                var notificationDto = CreateNotificationDto(item);
                await _notificationSender.SendEventNotificationToTgService(notificationDto, cancellationToken);

                AddNotificationToContext(item, notificationType);
                await _dbContext.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Sent {Type} notification for event {EventId} to user {UserId}",
                    notificationType, item.Event.Id, item.Participant.UserId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process notification for user {UserId} and event {EventId}",
                    item.Participant.UserId, item.Event.Id);
            }
        }
    }

    private SendToTelegramEventDto CreateNotificationDto(ParticipantNotificationInfo item)
    {
        return new SendToTelegramEventDto(
            EventId: item.Event.Id,
            Name: item.Event.Name ?? "Название мероприятия",
            Description: item.Event.Description ?? "Описание отсутствует",
            TimeStart: item.Event.TimeStart,
            TimeEnd: item.Event.TimeEnd,
            Type: item.Event.Type,
            Location: item.Event.Location ?? "Место не указано",
            TelegramUserId: item.UserTelegramId
        );
    }

    private void AddNotificationToContext(ParticipantNotificationInfo item, NotificationTypeEnum notificationType)
    {
        _dbContext.EventNotifications.Add(new EventNotificationsEntity
        {
            Id = Guid.NewGuid(),
            UserId = item.Participant.UserId,
            EventId = item.Participant.EventId,
            NotificationType = notificationType,
          //  CreatedAt = DateTime.UtcNow
        });
    }

    private class ParticipantNotificationInfo
    {
        public EventEntity Event { get; set; }
        public EventParticipantEntity Participant { get; set; }
        public long UserTelegramId { get; set; }
    }
}