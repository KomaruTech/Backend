using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TemplateService.Application.Event.DTOs;

namespace TemplateService.Telegram.Services;

public class EventsCommandHandler : ITelegramUpdateHandler
{
    private readonly ILogger<EventsCommandHandler> _logger;
    private readonly ITelegramBotClient _botClient;
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public EventsCommandHandler(
        ITelegramBotClient botClient,
        ILogger<EventsCommandHandler> logger,
        HttpClient httpClient,
        IConfiguration configuration)
    {
        _botClient = botClient;
        _logger = logger;
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        if (update.Type == UpdateType.Message && update.Message?.Text == "/events")
        {
            var user = update.Message.From;
            if (user == null) return;

            long telegramId = user.Id;

            _logger.LogInformation("Запрос ближайших ивентов: {UserId}", telegramId);

            try
            {
                string apiUrl = _configuration["ApiEndpoints:UpcomingEvents"]
                    ?? "http://template_api:5124/api/v1/events/upcoming";

                var request = new HttpRequestMessage(HttpMethod.Get, $"{apiUrl}?telegramUserId={telegramId}");
                request.Headers.Add("X-TG-API-Key", _configuration["X-TG-Api-Key"]!);

                var response = await _httpClient.SendAsync(request, cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    var events = await response.Content.ReadFromJsonAsync<List<EventDto>>(cancellationToken: cancellationToken);

                    if (events == null || events.Count == 0)
                    {
                        await botClient.SendTextMessageAsync(
                            telegramId,
                            "🎉 У вас нет предстоящих мероприятий!",
                            cancellationToken: cancellationToken);
                        return;
                    }

                    var message = "🎯 *Ваши ближайшие мероприятия:*\n\n";
                    foreach (var e in events)
                    {
                        message += $"📌 *{WebUtility.HtmlEncode(e.Name)}*\n";
                        message += $"⏰ {e.TimeStart:dd.MM.yyyy HH:mm}\n";
                        message += $"📍 {WebUtility.HtmlEncode(e.Location ?? "Место уточняется")}\n";
                        message += $"🔹 Тип: {e.Type.ToString().ToLower()}\n\n";
                    }

                    await botClient.SendTextMessageAsync(
                        telegramId,
                        message,
                        parseMode: ParseMode.MarkdownV2,
                        cancellationToken: cancellationToken);
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning("Ошибка API: {StatusCode} - {Content}", response.StatusCode, errorContent);
                    await botClient.SendTextMessageAsync(
                        telegramId,
                        "⚠️ Не удалось получить информацию о мероприятиях",
                        cancellationToken: cancellationToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении мероприятий");
                await botClient.SendTextMessageAsync(
                    telegramId,
                    "❌ Произошла ошибка при обработке запроса",
                    cancellationToken: cancellationToken);
            }
        }
    }

    public async Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "Ошибка обработки команды /events");
        await Task.CompletedTask;
    }
}