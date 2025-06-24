using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Npgsql;

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

        var conStrBuilder = new NpgsqlConnectionStringBuilder(_configuration.GetConnectionString("TemplatePostgreSqlDatabase"))
        {
            //Password = _configuration.GetValue<string>("DbPassword")
        };

        options.UseNpgsql(conStrBuilder.ConnectionString, opt =>
        {
            opt.MigrationsHistoryTable("TMP_EFMigrationsHistory", _defaultSchema);
        });
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(_defaultSchema);
        base.OnModelCreating(modelBuilder);
    }
}