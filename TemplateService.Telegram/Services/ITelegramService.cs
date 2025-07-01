using Telegram.Bot;
using Telegram.Bot.Types;

namespace TemplateService.Telegram.Services;

public interface ITelegramService
{ 
    Task StartReceiving(CancellationToken cancellationToken);
}