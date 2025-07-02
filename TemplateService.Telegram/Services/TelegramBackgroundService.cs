using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace TemplateService.Telegram.Services;

public class TelegramBackgroundService : BackgroundService
{
    private readonly ILogger<TelegramBackgroundService> _logger;
    private readonly IServiceProvider _serviceProvider;

    public TelegramBackgroundService(
        ILogger<TelegramBackgroundService> logger,
        IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                _logger.LogInformation("Starting Telegram polling...");
                using var scope = _serviceProvider.CreateScope();
                var telegramService = scope.ServiceProvider.GetRequiredService<ITelegramService>();
                await telegramService.StartReceiving(stoppingToken);
                await Task.Delay(Timeout.Infinite, stoppingToken); // Ожидаем бесконечно
            }
            catch (OperationCanceledException)
            {
                break; // Корректное завершение
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Telegram polling error. Restarting in 10 seconds...");
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken); // Пауза перед перезапуском
            }
        }
        _logger.LogInformation("Telegram polling stopped");
    }
}