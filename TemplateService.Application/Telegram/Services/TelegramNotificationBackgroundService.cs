using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace TemplateService.Application.Telegram.Services;

public class TelegramNotificationBackgroundService : BackgroundService
{
    private readonly ILogger<TelegramNotificationBackgroundService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(1);

    public TelegramNotificationBackgroundService(
        ILogger<TelegramNotificationBackgroundService> logger,
        IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Telegram Notification Background Service is starting.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var notificationService = scope.ServiceProvider
                        .GetRequiredService<ITelegramNotificationService>();

                    await notificationService.SendDailyNotificationAsync(stoppingToken);
                    await notificationService.SendHourlyNotificationAsync(stoppingToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while checking for upcoming events");
            }

            await Task.Delay(_checkInterval, stoppingToken);
        }

        _logger.LogInformation("Telegram Notification Background Service is stopping.");
    }
}