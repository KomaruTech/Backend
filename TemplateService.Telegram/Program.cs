using Telegram.Bot;
using Telegram.Bot.Polling;
using TemplateService.Telegram.Services;

namespace TemplateService.Telegram;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Configuration
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("tmp-appsettings.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables();
        
        builder.Services.AddControllers();
        builder.Services.AddHttpClient();
        
        builder.Services.AddSingleton<ITelegramBotClient>(sp =>
        {
            var token = builder.Configuration["Telegram:BotToken"]!;
            return new TelegramBotClient(token);
        });

        // Добавляем HttpClient через фабрику
        builder.Services.AddHttpClient();

        // Регистрируем IUpdateHandler как Scoped или Transient (лучше Scoped)
        builder.Services.AddScoped<IUpdateHandler, StartDialogHandler>();
        builder.Services.AddScoped<INotificationService, NotificationService>();

        // TelegramService тоже делаем Scoped, т.к. зависит от IUpdateHandler
        builder.Services.AddScoped<ITelegramService>(sp =>
        {
            var logger = sp.GetRequiredService<ILogger<TelegramService>>();
            var startDialogHandler = sp.GetRequiredService<IUpdateHandler>();
            var token = builder.Configuration["Telegram:BotToken"]!;
    
            return new TelegramService(token, logger, startDialogHandler);
        });
        
        builder.Services.AddHostedService<TelegramBackgroundService>();

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseRouting();
        app.MapControllers();

        app.Run();
    }
}