using InformService.Atach.WebUI.Proto.Document;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace TemplateService.Infrastructure.Extensions;

public static class GrpcServiceCollectionExtensions
{
    public static IServiceCollection AddGrpcServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddGrpcClient<DocumentGrpcService.DocumentGrpcServiceClient>
            ((services, options) =>
            {
                options.Address = new Uri(configuration.GetValue<string>("GrpcEndpoints:CoreApi"));
            })
            .ConfigureChannel(options => options.MaxReceiveMessageSize = null);

        return services;
    }
}