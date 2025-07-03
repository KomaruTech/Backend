using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using TemplateService.Application.TelegramService;
using System.Threading;
using System.Threading.Tasks;

namespace TemplateService.Application.Telegram.Services;

public class TelegramNotificationSender : ITelegramNotificationSender
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<TelegramNotificationSender> _logger;
    private const string Url = "http://template_telegram:5125/api/v1/telegram/notifications/send";

    public TelegramNotificationSender(
        HttpClient httpClient,
        ILogger<TelegramNotificationSender> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task SendEventNotificationToTgService(SendToTelegramEventDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var timeLeft = dto.TimeStart - DateTime.UtcNow;
            var notificationRequest = new
            {
                ChatId = dto.TelegramUserId,
                Text = FormatNotificationMessage(dto, timeLeft),
                ParseMode = "HTML",
                DisableWebPagePreview = true,
                EventId = dto.EventId
            };

            var response = await _httpClient.PostAsJsonAsync(Url, notificationRequest, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogError("Failed to send Telegram notification. Status: {StatusCode}, Error: {Error}",
                    response.StatusCode, errorContent);
                throw new HttpRequestException($"Telegram notification failed with status {response.StatusCode}");
            }

            _logger.LogInformation("Notification sent to {TelegramUserId} for event {EventName}",
                dto.TelegramUserId, dto.Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending Telegram notification to {TelegramUserId}", dto.TelegramUserId);
            throw;
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