namespace TemplateService.API.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddCustomSwaggerGen(this IServiceCollection services)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));


        return services;
    }
}
