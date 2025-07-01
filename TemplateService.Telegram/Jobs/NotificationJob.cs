using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TemplateService.Telegram.Services;

namespace TemplateService.Telegram.Jobs;

public class NotificationJob : BackgroundService
{
    private readonly ILogger<NotificationJob> _logger;
    private readonly IServiceProvider _serviceProvider;

    public NotificationJob(IServiceProvider serviceProvider, ILogger<NotificationJob> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Служба уведомлений запущена");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var notificationService = scope.ServiceProvider.GetRequiredService<NotificationService>();

                // Добавляем await
                await notificationService.CheckEventsAndSendNotifications();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка в фоновой задаче уведомлений");
            }

            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }
}