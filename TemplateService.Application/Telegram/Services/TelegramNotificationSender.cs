using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using TemplateService.Application.TelegramService;

namespace TemplateService.Application.Telegram.Services;

public class TelegramNotificationSender : ITelegramNotificationSender
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<TelegramNotificationSender> _logger;
    private const string Url = "http://template_telegram:5125/api/v1/telegram/notifications/send"; // должен быть Telegram Bot API endpoint

    public TelegramNotificationSender(HttpClient httpClient, ILogger<TelegramNotificationSender> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task SendEventNotificationToTgService(SendToTelegramEventDto dto, CancellationToken cancellationToken)
    {
        try
        {
            // Отправляем POST запрос с DTO в теле
            var response = await _httpClient.PostAsJsonAsync(Url, dto, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to send Telegram notification. Status code: {StatusCode}", response.StatusCode);
            }
            else
            {
                _logger.LogInformation("Telegram notification sent successfully for event {EventName} to user {TelegramUserId}", dto.Name, dto.TelegramUserId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while sending Telegram notification for event {EventName} to user {TelegramUserId}", dto.Name, dto.TelegramUserId);
            throw;
        }
    }
}