using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace TemplateService.Infrastructure.Persistence;

public class TemplateDbContextFactory : IDesignTimeDbContextFactory<TemplateDbContext>
{
    public TemplateDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("tmp-appsettings.json")
            .AddJsonFile("tmp-appsettings.Development.json")
            .Build();

        var builder = new DbContextOptionsBuilder<TemplateDbContext>();
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        // Используем NpgsqlDataSourceBuilder для включения динамического JSON
        var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
        dataSourceBuilder.EnableDynamicJson();
        var dataSource = dataSourceBuilder.Build();

        builder.UseNpgsql(dataSource);

        return new TemplateDbContext(builder.Options);
    }
}