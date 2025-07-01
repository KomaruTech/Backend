using Telegram.Bot;
using TemplateService.Telegram.DTO;

namespace TemplateService.Telegram.Services;

public class NotificationService : INotificationService
{
    private readonly ILogger<NotificationService> _logger;
    private readonly ITelegramService _telegramService;
    private readonly ITelegramBotClient _botClient;

    public NotificationService(
        ILogger<NotificationService> logger,
        ITelegramBotClient botClient,
        ITelegramService telegramService
        )
    {
        _logger = logger;
        _botClient = botClient;
        _telegramService = telegramService;
    }

    public async Task SendNotification(SendToTelegramEventDto dto, CancellationToken cancellationToken)
    {
        var message = dto.Description;
        
        try
        {
            await _botClient.SendMessage(
                chatId: dto.TelegramUserId,
                text: message,
                cancellationToken: cancellationToken);

            _logger.LogInformation("Сообщение отправлено в чат {ChatId}", dto.TelegramUserId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при отправке сообщения в чат {ChatId}", dto.TelegramUserId);
        }
    }

}