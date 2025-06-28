using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TemplateService.Application.Event.DTOs;
using TemplateService.Application.Event.Queries;

public class EventReminderService : BackgroundService
{
    private readonly IServiceProvider _services;
    private readonly ILogger<EventReminderService> _logger;

    public EventReminderService(
        IServiceProvider services,
        ILogger<EventReminderService> logger)
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
                var telegramService = scope.ServiceProvider.GetRequiredService<TelegramNotificationService>();

                var now = DateTime.UtcNow;
                var events = await mediator.Send(new GetEventQuery(now));

                foreach (var evt in events)
                {
                    await ProcessEventReminders(evt, telegramService);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in event reminder service");
            }

            await Task.Delay(TimeSpan.FromMinutes(1), ct); // Проверяем каждую минуту
        }
    }

    private async Task ProcessEventReminders(EventDto evt, TelegramNotificationService telegramService)
    {
        var timeUntilEvent = evt.TimeStart - DateTime.UtcNow;

        // Напоминание за 1 день
        if (!evt.ReminderSent1Day && timeUntilEvent <= TimeSpan.FromDays(1))
        {
            foreach (var participantId in evt.ParticipantIds)
            {
                await telegramService.SendNotification(
                    participantId, // Здесь предполагается, что participantId = chatId
                    $"Напоминание: событие '{evt.Name}' через 1 день!\n" +
                    $"Время: {evt.TimeStart:dd.MM.yyyy HH:mm}\n" +
                    $"Место: {evt.Location}" + 
                    $"Описание: {evt.Location}");

            }

            // Обновляем флаг отправки
            await _mediator.Send(new MarkEventReminderSentCommand(evt.Id, is1DayReminder: true));
        }

        // Напоминание за 1 час
        if (!evt.ReminderSent1Hour && timeUntilEvent <= TimeSpan.FromHours(1))
        {
            foreach (var participantId in evt.ParticipantIds)
            {
                await telegramService.SendNotification(
                    participantId,
                    $"Скоро начнётся: '{evt.Title}' через 1 час!\n" +
                    $"Ссылка: {evt.Location}");
            }

            await _mediator.Send(new MarkEventReminderSentCommand(evt.Id, is1DayReminder: false));
        }
    }
}