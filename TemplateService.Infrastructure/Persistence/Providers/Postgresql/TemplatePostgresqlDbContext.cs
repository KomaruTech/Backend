using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Npgsql;
using TemplateService.Domain.Enums;

namespace TemplateService.Infrastructure.Persistence.Providers.Postgresql;

public class TemplatePostgresqlDbContext : TemplateDbContext
{
    private readonly IConfiguration _configuration;

    public TemplatePostgresqlDbContext(
        DbContextOptions<TemplatePostgresqlDbContext> options,
        IConfiguration configuration)
        : base(ChangeOptionsType<TemplateDbContext>(options))
    {
        _configuration = configuration;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        base.OnConfiguring(options);

        var conStrBuilder = new NpgsqlConnectionStringBuilder(_configuration.GetConnectionString("DefaultConnection"));

        options.UseNpgsql(conStrBuilder.ConnectionString, opt =>
        {
            opt.MigrationsHistoryTable("TMP_EFMigrationsHistory", _defaultSchema);
            opt.MigrationsAssembly("TemplateService.Infrastructure");
        });
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        
        modelBuilder.HasDefaultSchema(_defaultSchema);
       
        modelBuilder.HasPostgresEnum<EventTypeEnum>();
        modelBuilder.HasPostgresEnum<ApplicationStatusEnum>();
        modelBuilder.HasPostgresEnum<UserRoleEnum>();
        base.OnModelCreating(modelBuilder);
    }
}