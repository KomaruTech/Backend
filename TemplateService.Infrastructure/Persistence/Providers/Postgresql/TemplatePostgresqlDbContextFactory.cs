using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace TemplateService.Infrastructure.Persistence.Providers.Postgresql;

public class TemplatePostgresqlDbContextFactory : IDesignTimeDbContextFactory<TemplatePostgresqlDbContext>
{
    public TemplatePostgresqlDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("tmp-appsettings.json", optional: false)
            .AddJsonFile("tmp-appsettings.Development.json", optional: true)
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection");
        
        var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
        dataSourceBuilder.EnableDynamicJson();
        var dataSource = dataSourceBuilder.Build();

        var optionsBuilder = new DbContextOptionsBuilder<TemplatePostgresqlDbContext>();
        optionsBuilder.UseNpgsql(dataSource);

        return new TemplatePostgresqlDbContext(optionsBuilder.Options, configuration);
    }
}