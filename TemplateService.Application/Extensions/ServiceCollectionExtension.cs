using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace TemplateService.Application.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddTemplateApplication(this IServiceCollection services)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        services.AddAutoMapper(Assembly.GetExecutingAssembly());

        services.AddMediatR(opt =>
        {
            opt.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
        });

        return services;
    }
}
