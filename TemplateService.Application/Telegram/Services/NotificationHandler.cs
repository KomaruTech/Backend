using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramUpdate = Telegram.Bot.Types.Update;

namespace TemplateService.Telegram.Services;

public class NotificationHandler : ITelegramUpdateHandler
{
    private readonly ITelegramBotClient _botClient;
    private readonly ILogger<NotificationHandler> _logger;

    public NotificationHandler(
        ITelegramBotClient botClient,
        ILogger<NotificationHandler> logger)
    {
        _botClient = botClient;
        _logger = logger;
    }

    public async Task HandleUpdateAsync(ITelegramBotClient botClient, TelegramUpdate update, CancellationToken cancellationToken)
    {
        if (update.Type != UpdateType.Message || update.Message?.Text == null)
            return;

        var message = update.Message;
        var chatId = message.Chat.Id;

        try
        {
            switch (message.Text)
            {
                case "/notifications":
                    await HandleNotificationsCommand(chatId, cancellationToken);
                    break;

                case "/help":
                    await HandleHelpCommand(chatId, cancellationToken);
                    break;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing update");
            await botClient.SendTextMessageAsync(
                chatId,
                "⚠️ Произошла ошибка при обработке запроса",
                cancellationToken: cancellationToken);
        }
    }


    private async Task HandleNotificationsCommand(long chatId, CancellationToken ct)
    {
        await _botClient.SendTextMessageAsync(
            chatId,
            "🔔 Управление уведомлениями:\n\n" +
            "Вы автоматически получаете уведомления:\n" +
            "• За 24 часа до события\n" +
            "• За 1 час до события\n\n" +
            "Для изменения настроек уведомлений посетите наш веб-портал",
            parseMode: ParseMode.Html,
            cancellationToken: ct);
    }

    private async Task HandleHelpCommand(long chatId, CancellationToken ct)
    {
        await _botClient.SendTextMessageAsync(
            chatId,
            "ℹ️ <b>Доступные команды:</b>\n\n" +
            "/start - Начало работы\n" +
            "/notifications - Настройки уведомлений\n" +
            "/help - Справка",
            parseMode: ParseMode.Html,
            cancellationToken: ct);
    }

    public async Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "Telegram polling error");
        await Task.CompletedTask;
    }
}