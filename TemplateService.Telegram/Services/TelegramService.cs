using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;

namespace TemplateService.Telegram.Services;

public class TelegramService : ITelegramService
{
    private readonly ITelegramBotClient _botClient;
    private readonly ILogger<TelegramService> _logger;
    private readonly IUpdateHandler _startDialogHandler;

    public TelegramService(
        string token,
        ILogger<TelegramService> logger,
        IUpdateHandler startDialogHandler
        )
    {
        _logger = logger;
        _startDialogHandler = startDialogHandler;
        _botClient = new TelegramBotClient(token);
    }

    public async Task StartReceiving(CancellationToken cancellationToken)
    {
        var receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = new[] { UpdateType.Message }
        };

        _botClient.StartReceiving(
            _startDialogHandler,
            receiverOptions,
            cancellationToken
        );

        var me = await _botClient.GetMe(cancellationToken);
        _logger.LogInformation("Бот запущен. Username: {Username}", me.Username);
    }
}