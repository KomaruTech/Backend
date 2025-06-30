using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TemplateService.Infrastructure.Persistence;
using TemplateService.Infrastructure.Persistence.Providers.Postgresql;

namespace TemplateService.Infrastructure.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddTemplateInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        services.AddDbContext<TemplatePostgresqlDbContext>();
        services.AddScoped<TemplateDbContext>(provider => provider.GetRequiredService<TemplatePostgresqlDbContext>());

        return services;
    }
}