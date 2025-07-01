#nullable enable
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using TemplateService.Telegram.Services;

namespace TemplateService.Telegram;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Configuration
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables();

        var configuration = builder.Configuration;

        // Минимальный набор сервисов для бота
        builder.Services.AddLogging();
        builder.Services.AddHttpClient();

        // Регистрация Telegram Bot Client
        builder.Services.AddSingleton<ITelegramBotClient>(sp =>
        {
            var token = configuration["Telegram:BotToken"]!;
            return new TelegramBotClient(token);
        });

        // Регистрируем наши обработчики
        builder.Services.AddScoped<ITelegramUpdateHandler, StartDialogHandler>();
        builder.Services.AddScoped<ITelegramUpdateHandler, EventsCommandHandler>();

        // Регистрация сервиса уведомлений
        builder.Services.AddScoped<INotificationService, NotificationService>();

        // Регистрация TelegramService
        builder.Services.AddScoped<ITelegramService>(sp =>
        {
            var logger = sp.GetRequiredService<ILogger<TelegramService>>();
            var handlers = sp.GetServices<ITelegramUpdateHandler>();
            var token = configuration["Telegram:BotToken"]!;

            return new TelegramService(token, logger, handlers);
        });

        // Фоновый сервис для бота
        builder.Services.AddHostedService<TelegramBackgroundService>();

        var app = builder.Build();
        app.Run();
    }
}