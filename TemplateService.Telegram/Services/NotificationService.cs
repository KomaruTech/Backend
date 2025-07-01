using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using TemplateService.Telegram.DTO;

namespace TemplateService.Telegram.Services;

public class NotificationService : INotificationService
{
    private readonly ILogger<NotificationService> _logger;
    private readonly ITelegramBotClient _botClient;

    public NotificationService(
        ILogger<NotificationService> logger,
        ITelegramBotClient botClient)
    {
        _logger = logger;
        _botClient = botClient;
    }

    public async Task SendNotification(SendToTelegramEventDto dto, CancellationToken cancellationToken)
    {
        try
        {
            await _botClient.SendTextMessageAsync(
                chatId: dto.TelegramUserId,
                text: dto.Description,
                parseMode: ParseMode.MarkdownV2,
                cancellationToken: cancellationToken);

            _logger.LogInformation("Сообщение отправлено в чат {ChatId}", dto.TelegramUserId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при отправке сообщения в чат {ChatId}", dto.TelegramUserId);
        }
    }
}