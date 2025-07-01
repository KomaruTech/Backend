using TemplateService.Telegram.Services;

public class TelegramBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<TelegramBackgroundService> _logger;

    public TelegramBackgroundService(IServiceProvider serviceProvider, ILogger<TelegramBackgroundService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Starting Telegram polling...");

        using (var scope = _serviceProvider.CreateScope())
        {
            var telegramService = scope.ServiceProvider.GetRequiredService<ITelegramService>();

            try
            {
                await telegramService.StartReceiving(stoppingToken);
                await Task.Delay(Timeout.Infinite, stoppingToken); // держим сервис живым
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Telegram polling stopped with error");
            }
        }

        _logger.LogInformation("Telegram polling stopped");
    }
}