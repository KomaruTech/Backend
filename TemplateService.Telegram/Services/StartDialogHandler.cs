using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TemplateService.Telegram.Services;

public class StartDialogHandler : ITelegramUpdateHandler
{
    private readonly ILogger<StartDialogHandler> _logger;
    private readonly ITelegramBotClient _botClient;
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public StartDialogHandler(
        ITelegramBotClient botClient,
        ILogger<StartDialogHandler> logger,
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
        if (update.Type == UpdateType.Message && update.Message?.Text == "/start")
        {
            var user = update.Message!.From!;
            long telegramId = user.Id;
            string? username = user.Username;
            string firstName = WebUtility.HtmlEncode(user.FirstName ?? "Пользователь");

            _logger.LogInformation("Пользователь запустил /start: {FirstName} ({TelegramId})", firstName, telegramId);

            // Формируем красивое приветствие
            string welcomeMessage = $"""
                👋 Привет, <b>{firstName}</b>!

                Добро пожаловать в <i>Event Notification Service</i>!

                ✅ Теперь вы будете получать уведомления о:
                • Запланированных событиях
                • Изменениях в расписании
                • Важных обновлениях

                ✨ Используйте наш веб-портал для управления уведомлениями.
                """;

            // Отправка данных на API
            var payload = new
            {
                TelegramUserId = telegramId,
                TelegramUserName = username
            };

            try
            {
                string apiUrl = _configuration["ApiEndpoints:TelegramConnect"]
                    ?? "http://template_api:5124/api/v1/telegram/connect";

                var request = new HttpRequestMessage(HttpMethod.Post, apiUrl)
                {
                    Content = JsonContent.Create(payload)
                };

                request.Headers.Add("X-TG-API-Key", _configuration["X-TG-Api-Key"]!);

                var response = await _httpClient.SendAsync(request, cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    await botClient.SendTextMessageAsync(
                        chatId: telegramId,
                        text: welcomeMessage,
                        parseMode: ParseMode.Html,
                        cancellationToken: cancellationToken);
                }
                else
                {
                    await botClient.SendTextMessageAsync(
                        chatId: telegramId,
                        text: "❌ <b>Ошибка подключения</b>\nПопробуйте позже или обратитесь в поддержку",
                        parseMode: ParseMode.Html,
                        cancellationToken: cancellationToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при отправке данных на API");
                await botClient.SendTextMessageAsync(
                    chatId: telegramId,
                    text: "⚠️ <b>Системная ошибка</b>\nСообщите администратору об этой проблеме",
                    parseMode: ParseMode.Html,
                    cancellationToken: cancellationToken);
            }
        }
    }

    public async Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
    }
}