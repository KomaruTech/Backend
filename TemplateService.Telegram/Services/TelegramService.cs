using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TemplateService.Telegram.Services;

public class TelegramService : ITelegramService
{
    private readonly ITelegramBotClient _botClient;
    private readonly ILogger<TelegramService> _logger;
    private readonly IEnumerable<ITelegramUpdateHandler> _handlers;

    public TelegramService(
        string token,
        ILogger<TelegramService> logger,
        IEnumerable<ITelegramUpdateHandler> handlers)
    {
        _logger = logger;
        _handlers = handlers;
        _botClient = new TelegramBotClient(token);
    }

    public async Task StartReceiving(CancellationToken cancellationToken)
    {
        var receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = new[] { UpdateType.Message }
        };

        _botClient.StartReceiving(
            updateHandler: HandleUpdateAsync,
            errorHandler: HandlePollingErrorAsync,
            receiverOptions: receiverOptions,
            cancellationToken: cancellationToken
        );

        var me = await _botClient.GetMe(cancellationToken);
        _logger.LogInformation("Бот запущен. Username: {Username}", me.Username);
    }

    private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        foreach (var handler in _handlers)
        {
            await handler.HandleUpdateAsync(botClient, update, cancellationToken);
        }
    }

    private async Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        foreach (var handler in _handlers)
        {
            await handler.HandlePollingErrorAsync(botClient, exception, cancellationToken);
        }
    }
}