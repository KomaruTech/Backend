using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using TemplateService.Application.TelegramService;
using System.Threading;
using System.Threading.Tasks;

namespace TemplateService.Application.Telegram.Services;

public class TelegramNotificationSender : ITelegramNotificationSender
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<TelegramNotificationSender> _logger;
    private readonly string _notificationUrl;
    private readonly TimeSpan _logInterval = TimeSpan.FromMinutes(1);

    public TelegramNotificationSender(
        HttpClient httpClient,
        ILogger<TelegramNotificationSender> logger,
        IConfiguration configuration)
    {
        _httpClient = httpClient;
        _logger = logger;
        _notificationUrl = configuration["Telegram:NotificationUrl"]
            ?? "http://template_api:5124/api/v1/telegram/notifications/send";

        _httpClient.DefaultRequestHeaders.Accept.Add(
            new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
    }

    public async Task SendEventNotificationToTgService(SendToTelegramEventDto dto, CancellationToken cancellationToken)
    {
        try
        {
            // Валидация и подстановка значений по умолчанию
            dto = ValidateAndSetDefaults(dto);

            // Логирование времени до события
            var timeUntilEvent = dto.TimeStart - DateTime.UtcNow;
            LogTimeUntilNotification(dto, timeUntilEvent);

            // Формируем запрос в формате, ожидаемом API
            var apiRequest = new
            {
                // Обязательные поля
                Name = dto.Name,
                Description = dto.Description,

                // Дополнительные поля
                EventId = dto.EventId,
                TimeStart = dto.TimeStart,
                TimeEnd = dto.TimeEnd,
                Type = dto.Type.ToString(),
                Location = dto.Location,
                TelegramUserId = dto.TelegramUserId,

                // Форматированное сообщение для Telegram
                Text = FormatNotificationMessage(dto, timeUntilEvent),
                ParseMode = "HTML"
            };

            _logger.LogDebug("Sending notification request: {@Request}", apiRequest);

            var response = await _httpClient.PostAsJsonAsync(
                _notificationUrl,
                apiRequest,
                cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogError("Failed to send notification. Status: {StatusCode}, Error: {Error}",
                    response.StatusCode, errorContent);
                throw new HttpRequestException($"Notification failed with status {response.StatusCode}");
            }

            _logger.LogInformation("Notification successfully sent to {TelegramUserId}", dto.TelegramUserId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending notification for event {EventId}", dto.EventId);
            throw;
        }
    }

    private SendToTelegramEventDto ValidateAndSetDefaults(SendToTelegramEventDto dto)
    {
        // Проверка и подстановка значений по умолчанию
        if (string.IsNullOrWhiteSpace(dto.Name))
        {
            _logger.LogWarning("Event {EventId} has empty name", dto.EventId);
            dto = dto with { Name = "Уведомление о событии" };
        }

        if (string.IsNullOrWhiteSpace(dto.Description))
        {
            _logger.LogWarning("Event {EventId} has empty description", dto.EventId);
            dto = dto with { Description = "Описание отсутствует" };
        }

        if (string.IsNullOrWhiteSpace(dto.Location))
        {
            dto = dto with { Location = "Место не указано" };
        }

        return dto;
    }

    private void LogTimeUntilNotification(SendToTelegramEventDto dto, TimeSpan timeUntilEvent)
    {
        if (timeUntilEvent.TotalMinutes <= 0) return;

        if (timeUntilEvent.TotalDays >= 1)
        {
            _logger.LogInformation(
                "Event '{EventName}' (ID: {EventId}) will notify in {Days} days {Hours} hours {Minutes} minutes",
                dto.Name, dto.EventId,
                timeUntilEvent.Days, timeUntilEvent.Hours, timeUntilEvent.Minutes);
        }
        else if (timeUntilEvent.TotalHours >= 1)
        {
            _logger.LogInformation(
                "Event '{EventName}' (ID: {EventId}) will notify in {Hours} hours {Minutes} minutes",
                dto.Name, dto.EventId,
                timeUntilEvent.Hours, timeUntilEvent.Minutes);
        }
        else
        {
            _logger.LogInformation(
                "Event '{EventName}' (ID: {EventId}) will notify in {Minutes} minutes",
                dto.Name, dto.EventId,
                timeUntilEvent.Minutes);
        }
    }

    private string FormatNotificationMessage(SendToTelegramEventDto dto, TimeSpan timeLeft)
    {
        string timeLeftText = timeLeft.TotalHours >= 23
            ? $"Через 1 день ({dto.TimeStart:dd.MM.yyyy HH:mm})"
            : $"Через 1 час ({dto.TimeStart:HH:mm})";

        return $"{(timeLeft.TotalHours >= 23 ? "📅" : "⏰")} <b>{(timeLeft.TotalHours >= 23 ? "Напоминание" : "Скоро начнётся")}</b>\n\n" +
               $"<b>{dto.Name}</b>\n" +
               $"⏱ {timeLeftText}\n" +
               $"📍 {dto.Location ?? "Место не указано"}\n" +
               $"📝 <i>{dto.Description}</i>\n\n" +
               $"<a href=\"https://your-service.com/events/{dto.EventId}\">Подробнее</a>";
    }
}