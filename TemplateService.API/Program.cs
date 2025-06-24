using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.OpenApi.Models;
using System.Net;
using System.Reflection;
using TemplateService.API.Extensions;
using TemplateService.API.GrpcServices;
using TemplateService.Application.Extensions;
using TemplateService.Infrastructure.Extensions;
using TemplateService.Infrastructure.Persistence;

namespace TemplateService.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Configuration.AddCustomConfiguration();

            builder.WebHost.ConfigureKestrel(options =>
            {
                var (httpPort, grpcPort) = GetDefinedPorts(builder.Configuration);

                options.Listen(IPAddress.Any, httpPort, listenOptions =>
                {
                    listenOptions.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http1AndHttp2;
                });

                options.Listen(IPAddress.Any, grpcPort, listenOptions =>
                {
                    listenOptions.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http2;
                });
            });

            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddCors();

            builder.Services.AddSwaggerGen(config =>
            {
                config.SwaggerDoc("v1", new OpenApiInfo { Title = "TemplateService.Api", Version = "v1" });

                // Add swagger documentation
                var currentAssembly = Assembly.GetExecutingAssembly();
                var xmlDocs = currentAssembly.GetReferencedAssemblies()
                .Union(new[] { currentAssembly.GetName() })
                .Select(a => Path.Combine(Path.GetDirectoryName(currentAssembly.Location), $"{a.Name}.xml"))
                .Where(f => File.Exists(f)).ToList();

                // Генерация XML-документации
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

                if (File.Exists(xmlPath))
                {
                    config.IncludeXmlComments(xmlPath);
                }
                else
                {
                    // Логируем предупреждение, если файл не найден
                    var logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<Program>();
                    logger.LogWarning($"XML documentation file not found at: {xmlPath}");
                }


                xmlDocs.ForEach(xmlDoc =>
                {
                    config.IncludeXmlComments(xmlDoc, includeControllerXmlComments: true);
                });

                var securitySchema = new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                };

                config.AddSecurityDefinition("Bearer", securitySchema);

                var securityRequirement = new OpenApiSecurityRequirement
                {
                    { securitySchema, new[] { "Bearer" } }
                };

                config.AddSecurityRequirement(securityRequirement);
            });

            builder.Services.AddTemplateInfrastructure(builder.Configuration);
            builder.Services.AddTemplateApplication();

            #region gRPC
            builder.Services.AddGrpc(options =>
            {
                options.EnableDetailedErrors = true;
            });

            builder.Services.AddGrpcServices(builder.Configuration);

            builder.Services.AddGrpcReflection();
            #endregion

            var app = builder.Build();

            MigrateDatabase(app.Services);

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

           

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "TemplateService.Api v1");
                c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
            });

            app.UseHttpsRedirection();

            app.UseDefaultFiles();
            app.UseStaticFiles();
            

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.MapGrpcReflectionService();
            app.MapGrpcService<TmpGrpcService>().EnableGrpcWeb();
            app.MapGrpcHealthChecksService();

            app.Run();
        }

        private static (int httpPort, int grpcPort) GetDefinedPorts(IConfiguration config)
        {
            var grpcPort = config.GetValue<int>("GRPC_PORT");
            var port = config.GetValue<int>("WEB_PORT");
            return (port, grpcPort);
        }

        private static void MigrateDatabase(IServiceProvider service)
        {
            using var scope = service.CreateScope();
            var services = scope.ServiceProvider;

            try
            {
                var сontext = services.GetRequiredService<TemplateDbContext>();

                сontext.Migrate();
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}