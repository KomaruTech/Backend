using Microsoft.EntityFrameworkCore;
using Npgsql;
using TemplateService.Application.Extensions;
using TemplateService.Application.TelegramService;
using TemplateService.Infrastructure.Extensions;
using TemplateService.Infrastructure.Persistence.Providers.Postgresql;

namespace TemplateService.Worker;

class Program
{
    static void Main(string[] args)
    {
        var host = Host.CreateDefaultBuilder(args)
            .ConfigureServices((context, services) =>
            {
                var connectionString = context.Configuration.GetConnectionString("DefaultConnection");

                var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
                dataSourceBuilder.EnableDynamicJson();
                var dataSource = dataSourceBuilder.Build();

                services.AddSingleton(dataSource);

                services.AddDbContext<TemplatePostgresqlDbContext>((provider, options) =>
                {
                    var ds = provider.GetRequiredService<NpgsqlDataSource>();
                    options.UseNpgsql(ds);
                });
                services.AddHttpClient();
                services.AddScoped<ITelegramNotificationSender, TelegramNotificationSender>();
                services.AddScoped<ITelegramNotificationService, TelegramNotificationService>();
                
                services.AddTemplateInfrastructure(context.Configuration);
                services.AddTemplateApplication();
                
                services.AddHostedService<TelegramNotificationWorker>();
            })
            .Build();

        host.Run();
    }
}