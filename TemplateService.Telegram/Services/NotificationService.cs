using System.Text;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
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
        try
        {
            var message = $"{"*📢 Предстоящее мероприятие\\!*"}\n\n" +
                          $"{"*Название:*"} {Escape(dto.Name)}\n" +
                          $"{"*Тип:*"} {Escape(dto.Type.ToString())}\n" +
                          $"{"*Описание:*"} {Escape(dto.Description)}\n\n" +
                          $"{"*Начало:*"} {Escape(dto.TimeStart.ToString("dd.MM.yyyy HH:mm"))}\n" +
                          (dto.TimeEnd != null ? $"{"*Окончание:*"} {Escape(dto.TimeEnd.Value.ToString("dd.MM.yyyy HH:mm"))}\n" : "") +
                          ($"{"*Место:*"} {Escape(dto.Location)}\n");

            await _botClient.SendMessage(
                chatId: dto.TelegramUserId,
                text: message,
                parseMode: ParseMode.MarkdownV2,
                cancellationToken: cancellationToken);

            _logger.LogInformation("Сообщение отправлено в чат {ChatId}", dto.TelegramUserId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при отправке сообщения в чат {ChatId}", dto.TelegramUserId);
        }
    }

    /// <summary>
    /// Экранирует специальные символы MarkdownV2.
    /// </summary>
    private static string Escape(string text)
    {
        if (string.IsNullOrEmpty(text))
            return string.Empty;

        var specialChars = new[] { "\\", "_", "*", "[", "]", "(", ")", "~", "`", ">", "#", "+", "-", "=", "|", "{", "}", ".", "!" };

        var escaped = new StringBuilder(text.Length);
        foreach (var ch in text)
        {
            if (specialChars.Contains(ch.ToString()))
                escaped.Append('\\');
            escaped.Append(ch);
        }

        return escaped.ToString();
    }
}