using System.Net.Http.Json;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace TemplateService.Telegram.Services;

public class StartDialogHandler : IUpdateHandler
{
    private readonly ILogger<StartDialogHandler> _logger;
    private readonly ITelegramBotClient _botClient;
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public StartDialogHandler(
        ITelegramBotClient botClient, 
        ILogger<StartDialogHandler> logger,
        HttpClient httpClient,
        IConfiguration configuration
        )
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
            var telegramId = update.Message!.From!.Id;
            string? username = update.Message!.From!.Username;

            _logger.LogInformation("Пользователь запустил /start: {Username} ({TelegramId})", username, telegramId);

            // Отправка данных на API
            var payload = new
            {
                TelegramUserId = telegramId,
                TelegramUserName = username
            };

            try
            {
                
                // Получаем ключ из конфигурации (пример)
                string apiKey = _configuration["X-TG-Api-Key"]!; // или другой путь к ключу

                var request = new HttpRequestMessage(HttpMethod.Post, "http://template_api:5124/api/v1/telegram/connect")
                {
                    Content = JsonContent.Create(payload)
                };
                
                // Добавляем заголовок
                request.Headers.Add("X-TG-Api-Key", apiKey);
                
                var response = await _httpClient.SendAsync(request, cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    await _botClient.SendMessage(telegramId, "Вы успешно подключены!", cancellationToken: cancellationToken);
                }
                else
                {
                    await _botClient.SendMessage(telegramId, "Произошла ошибка при подключении.", cancellationToken: cancellationToken);
                    _logger.LogWarning("Не удалось подключить пользователя {Username}. Статус: {StatusCode}", username, response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при отправке данных на API для пользователя {Username}", username);
                await _botClient.SendMessage(telegramId, "Внутренняя ошибка при подключении.", cancellationToken: cancellationToken);
            }
        }
    }
    
    public Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, HandleErrorSource source, CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "Ошибка при получении обновлений Telegram");
        return Task.CompletedTask;
    }
}