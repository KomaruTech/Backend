// Services/TelegramNotificationService.cs
using Telegram.Bot;
using Telegram.Bot.Types;

public class TelegramNotificationService
{
    private readonly ITelegramBotClient _botClient;
    private readonly ILogger<TelegramNotificationService> _logger;

    public TelegramNotificationService(
        ITelegramBotClient botClient,
        ILogger<TelegramNotificationService> logger)
    {
        _botClient = botClient;
        _logger = logger;
    }

    public async Task SendNotification(long chatId, string message)
    {
        try
        {
            await _botClient.SendTextMessageAsync(
                chatId: chatId,
                text: message);

            _logger.LogInformation($"Notification sent to chat {chatId}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to send notification to chat {chatId}");
        }
    }
}