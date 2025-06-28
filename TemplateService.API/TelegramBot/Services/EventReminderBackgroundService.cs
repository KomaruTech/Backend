// Services/EventReminderBackgroundService.cs
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using TemplateService.Application.Event.DTOs;

public class EventReminderBackgroundService : BackgroundService
{
    private readonly IServiceProvider _services;
    private readonly ILogger<EventReminderBackgroundService> _logger;

    public EventReminderBackgroundService(
        IServiceProvider services,
        ILogger<EventReminderBackgroundService> logger)
    {
        _services = services;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            try
            {
                using var scope = _services.CreateScope();
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                var notificationService = scope.ServiceProvider.GetRequiredService<EventNotificationService>();

                var now = DateTime.UtcNow;
                var upcomingEvents = await mediator.Send(new GetEventsForRemindersQuery(now));

                foreach (var evt in upcomingEvents)
                {
                    await ProcessEventReminders(evt, notificationService, mediator);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in reminder service");
            }

            await Task.Delay(TimeSpan.FromMinutes(1), ct); // Проверка каждую минуту
        }
    }

    private async Task ProcessEventReminders(
        EventDto evt,
        EventNotificationService notificationService,
        IMediator mediator)
    {
        var timeUntilEvent = evt.TimeStart - DateTime.UtcNow;

        // Напоминание за 1 день
        if (ShouldSend1DayReminder(evt, timeUntilEvent))
        {
            await notificationService.SendEventReminder(
                evt.Id,
                evt.Name,
                evt.TimeStart,
                evt.Location,
                evt.ParticipantIds,
                "1day");

            await mediator.Send(new MarkReminderSentCommand(evt.Id, is1DayReminder: true));
        }

        // Напоминание за 1 час
        if (ShouldSend1HourReminder(evt, timeUntilEvent))
        {
            await notificationService.SendEventReminder(
                evt.Id,
                evt.Name,
                evt.TimeStart,
                evt.Location,
                evt.ParticipantIds,
                "1hour");

            await mediator.Send(new MarkReminderSentCommand(evt.Id, is1DayReminder: false));
        }
    }

    private bool ShouldSend1DayReminder(EventDto evt, TimeSpan timeUntilEvent)
    {
        return evt.NotificationsEnabled &&
               !evt.Reminder1DaySent &&
               timeUntilEvent <= TimeSpan.FromDays(1) &&
               timeUntilEvent > TimeSpan.FromHours(23);
    }

    private bool ShouldSend1HourReminder(EventDto evt, TimeSpan timeUntilEvent)
    {
        return evt.NotificationsEnabled &&
               !evt.Reminder1HourSent &&
               timeUntilEvent <= TimeSpan.FromHours(1) &&
               timeUntilEvent > TimeSpan.FromMinutes(59);
    }
}