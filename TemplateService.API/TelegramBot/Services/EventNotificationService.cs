// Services/EventNotificationService.cs
using Telegram.Bot;
using Telegram.Bot.Types;

public class EventNotificationService
{
    private readonly ITelegramBotClient _botClient;
    private readonly ILogger<EventNotificationService> _logger;

    public EventNotificationService(
        ITelegramBotClient botClient,
        ILogger<EventNotificationService> logger)
    {
        _botClient = botClient;
        _logger = logger;
    }

    public async Task SendEventReminder(
        Guid eventId,
        string eventName,
        DateTime eventTime,
        string location,
        IEnumerable<Guid> participantChatIds,
        string reminderType)
    {
        var message = reminderType switch
        {
            "1day" => $"⏰ Напоминание: мероприятие '{eventName}' через 1 день!\n" +
                     $"📅 Дата: {eventTime:dd.MM.yyyy}\n" +
                     $"🕒 Время: {eventTime:HH:mm}\n" +
                     $"📍 Место: {location ?? "не указано"}",

            "1hour" => $"🔔 Скоро начало: '{eventName}' через 1 час!\n" +
                      $"⏱ Начало в {eventTime:HH:mm}",

            _ => throw new ArgumentException("Invalid reminder type")
        };

        foreach (var chatId in participantChatIds)
        {
            try
            {
                await _botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: message);

                _logger.LogInformation($"Sent {reminderType} reminder for event {eventId} to {chatId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send reminder to {chatId}");
            }
        }
    }
}