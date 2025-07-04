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

                options.Listen(IPAddress.Any, httpPort, listenOptions => { listenOptions.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http1AndHttp2; });
            });

            builder.Services
                .AddControllers()
                .AddJsonOptions(options => { options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()); });
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddCors();
            builder.Services.AddHttpContextAccessor();

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


            builder.Services.AddSwaggerGen(config =>
            {
                config.SwaggerDoc("v1", new OpenApiInfo { Title = "TemplateService.Api", Version = "v1" });

                // Add swagger documentation
                var currentAssembly = Assembly.GetExecutingAssembly();
                var xmlDocs = currentAssembly.GetReferencedAssemblies()
                    .Union(new[] { currentAssembly.GetName() })
                    .Select(a => Path.Combine(Path.GetDirectoryName(currentAssembly.Location), $"{a.Name}.xml"))
                    .Where(f => File.Exists(f)).ToList();

                xmlDocs.ForEach(xmlDoc => { config.IncludeXmlComments(xmlDoc, includeControllerXmlComments: true); });

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
                        new string[] { }
                    }
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

            builder.Services.AddHttpClient();

            builder.Services.AddScoped<IPasswordHelper, PasswordHelper>();
            builder.Services.AddScoped<ITokenService, TokenService>();
            builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
            builder.Services.AddScoped<IUserValidationService, UserValidationService>();
            builder.Services.AddScoped<IEventValidationService, EventValidationService>();
            builder.Services.AddScoped<IUserHelperService, UserHelperService>();
            builder.Services.AddScoped<ITeamValidationService, TeamValidationService>();
            builder.Services.AddAutoMapper(typeof(AvatarUrlResolver).Assembly);

            builder.Services.AddSingleton<IPasswordHelper, PasswordHelper>();

            // Временно (Разрешены любые CORS)
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

            MigrateDatabase(app.Services);

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Временно (Разрешены любые CORS)
            app.UseCors();

            // Миддлвейр для обработки ошибок
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
            var port = config.GetValue<int>("WEB_PORT");
            return port;
        }

        private static void MigrateDatabase(IServiceProvider service)
        {
            using var scope = service.CreateScope();
            var services = scope.ServiceProvider;

            var context = services.GetRequiredService<TemplateDbContext>();

            context.Migrate();
        }
    }
}