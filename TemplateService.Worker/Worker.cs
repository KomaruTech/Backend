using TemplateService.Application.TelegramService;

namespace TemplateService.Worker;

public class TelegramNotificationWorker : BackgroundService
{
    private readonly ILogger<TelegramNotificationWorker> _logger;
    private readonly IServiceScopeFactory _scopeFactory;

    public TelegramNotificationWorker(ILogger<TelegramNotificationWorker> logger, IServiceScopeFactory scopeFactory)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _scopeFactory.CreateScope();
            var eventService = scope.ServiceProvider.GetRequiredService<ITelegramNotificationService>();

            await eventService.SendDailyNotificationAsync(stoppingToken);

            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
        }
    }
}