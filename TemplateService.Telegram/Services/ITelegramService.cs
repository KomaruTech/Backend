using System.Threading;
using System.Threading.Tasks;

namespace TemplateService.Telegram.Services;

public interface ITelegramService
{
    Task StartReceiving(CancellationToken cancellationToken);
}