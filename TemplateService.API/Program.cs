#nullable enable

using System.Net;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Npgsql;
using TemplateService.API.Extensions;
using TemplateService.API.Middleware;
using TemplateService.Application.Auth.Services;
using TemplateService.Application.Event.Services;
using TemplateService.Application.Extensions;
using TemplateService.Application.PasswordService;
using TemplateService.Application.Teams.Services;
using TemplateService.Application.Telegram.Services;
using TemplateService.Application.TokenService;
using TemplateService.Application.User.Services;
using TemplateService.Infrastructure.Extensions;
using TemplateService.Infrastructure.Persistence;
using TemplateService.Infrastructure.Persistence.Providers.Postgresql;

namespace TemplateService.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

            var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
            dataSourceBuilder.EnableDynamicJson();
            var dataSource = dataSourceBuilder.Build();

            builder.Services.AddSingleton(dataSource);
            builder.Services.AddDbContext<TemplateDbContext>((provider, options) =>
            {
                var ds = provider.GetRequiredService<NpgsqlDataSource>();
                options.UseNpgsql(ds);
            });

            builder.Configuration.AddCustomConfiguration();

            builder.WebHost.ConfigureKestrel(options =>
            {
                var httpPort = GetDefinedPorts(builder.Configuration);
                options.Listen(IPAddress.Any, httpPort, listenOptions =>
                {
                    listenOptions.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http1AndHttp2;
                });
            });

            // Регистрация сервисов
            builder.Services
                .AddControllers()
                .AddJsonOptions(options => { options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()); });

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddHttpClient();

            // Регистрация DbContext
            builder.Services.AddDbContext<TemplatePostgresqlDbContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")),
                contextLifetime: ServiceLifetime.Scoped,
                optionsLifetime: ServiceLifetime.Singleton);

            // Регистрация Telegram сервисов
            builder.Services.AddScoped<ITelegramNotificationSender, TelegramNotificationSender>();
            builder.Services.AddScoped<ITelegramNotificationService, TelegramNotificationService>();
            builder.Services.AddSingleton<IHostedService>(provider =>
                new TelegramNotificationBackgroundService(
                provider.GetRequiredService<ILogger<TelegramNotificationBackgroundService>>(),
                provider.GetRequiredService<IServiceProvider>()));

            // Регистрация аутентификации
            var jwtSettings = builder.Configuration.GetSection("Jwt");
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidAudience = jwtSettings["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]))
                };
            });

            // Настройка Swagger
            builder.Services.AddSwaggerGen(config =>
            {
                config.SwaggerDoc("v1", new OpenApiInfo { Title = "TemplateService.Api", Version = "v1" });

                var currentAssembly = Assembly.GetExecutingAssembly();
                var xmlDocs = currentAssembly.GetReferencedAssemblies()
                    .Union(new[] { currentAssembly.GetName() })
                    .Select(a => Path.Combine(Path.GetDirectoryName(currentAssembly.Location), $"{a.Name}.xml"))
                    .Where(f => File.Exists(f)).ToList();

                xmlDocs.ForEach(xmlDoc => { config.IncludeXmlComments(xmlDoc, includeControllerXmlComments: true); });

                // Настройка безопасности для Telegram API Key
                config.AddSecurityDefinition("X-TG-API-Key", new OpenApiSecurityScheme
                {
                    Name = "X-TG-API-Key",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "ApiKey",
                    In = ParameterLocation.Header,
                    Description = "Секретный ключ для доступа к Telegram API"
                });

                config.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "X-TG-API-Key"
                            }
                        },
                        Array.Empty<string>()
                    }
                });

                // Настройка JWT авторизации
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
                config.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    { securitySchema, new[] { "Bearer" } }
                });
            });

            // Регистрация application и infrastructure сервисов
            builder.Services.AddTemplateInfrastructure(builder.Configuration);
            builder.Services.AddTemplateApplication();

            // Регистрация других сервисов
            builder.Services.AddScoped<IPasswordHelper, PasswordHelper>();
            builder.Services.AddScoped<ITokenService, TokenService>();
            builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
            builder.Services.AddScoped<IUserValidationService, UserValidationService>();
            builder.Services.AddScoped<IEventValidationService, EventValidationService>();
            builder.Services.AddScoped<IUserHelperService, UserHelperService>();
            builder.Services.AddScoped<ITeamValidationService, TeamValidationService>();
            builder.Services.AddAutoMapper(typeof(AvatarUrlResolver).Assembly);

            // Настройка CORS (временно разрешены все запросы)
            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy =>
                {
                    policy.AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });

            var app = builder.Build();

            // Применение миграций
            MigrateDatabase(app.Services);

            // Конфигурация pipeline
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors();
            app.UseMiddleware<ExceptionHandlingMiddleware>();

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

            app.Run();
        }

        private static int GetDefinedPorts(IConfiguration config)
        {
            return config.GetValue<int>("WEB_PORT");
        }

        private static void MigrateDatabase(IServiceProvider service)
        {
            using var scope = service.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<TemplateDbContext>();
            context.Migrate();
        }
    }
}