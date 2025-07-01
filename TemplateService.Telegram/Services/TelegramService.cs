#nullable enable
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TemplateService.Telegram.Services;

public class TelegramService
{
    private readonly ITelegramBotClient _botClient;
    private readonly ILogger<TelegramService> _logger;

    public TelegramService(string botToken, ILogger<TelegramService> logger)
    {
        _logger = logger;
        _botClient = new TelegramBotClient(botToken);
    }

    // Добавляем async и возвращаем Task
    public async Task SendMessage(long chatId, string message)
    {
        try
        {
            await _botClient.SendTextMessageAsync(
                chatId: chatId,
                text: message);

            _logger.LogInformation("Сообщение отправлено в чат {ChatId}", chatId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при отправке сообщения в чат {ChatId}", chatId);
        }
    }
}