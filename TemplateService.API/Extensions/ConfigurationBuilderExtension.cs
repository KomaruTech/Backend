namespace TemplateService.API.Extensions;

public static class ConfigurationBuilderExtension
{
    public static IConfigurationBuilder AddCustomConfiguration(this IConfigurationBuilder conf)
    {
        if (conf == null)
            throw new ArgumentNullException(nameof(conf));

        var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

        conf.SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("tmp-appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"tmp-appsettings.{env}.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables();

        return conf;
    }
}
