using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Npgsql;
using TemplateService.Domain.Enums;
using TemplateService.Infrastructure.Persistence;


namespace TemplateService.Infrastructure.Persistence.Providers.Postgresql;

public class TemplatePostgresqlDbContext : TemplateDbContext
{
    public TemplatePostgresqlDbContext(
        DbContextOptions<TemplatePostgresqlDbContext> options,
        IConfiguration configuration)
        : base(options, configuration)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        if (!options.IsConfigured)
        {
            options.UseNpgsql(Configuration.GetConnectionString("PostgreSqlDatabase"), opt =>
            {
                opt.MigrationsHistoryTable("TMP_EFMigrationsHistory", _defaultSchema);
                opt.MigrationsAssembly("TemplateService.Infrastructure");
            });
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasPostgresEnum<ApplicationStatusEnum>();
        modelBuilder.HasPostgresEnum<EventTypeEnum>();
    }
}