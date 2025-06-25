using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TemplateService.Domain.Entities;

namespace TemplateService.Infrastructure.Persistence;

public class TemplateDbContext : DbContext
{
    protected readonly string _defaultSchema = "XXATACH_TMP";

    public TemplateDbContext(DbContextOptions<TemplateDbContext> options)
        : base(options)
    {
    }

    public DbSet<DocumentEntity>  Documents { get; set; }
    public DbSet<MetaEntity>  Metas { get; set; }
    public DbSet<UserEntity> Users { get; set; }

    public void Migrate()
    {
        try
        {
            // Получаем путь к папке API-проекта
            var apiProjectPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "..",
                "TemplateService.API");

            var configuration = new ConfigurationBuilder()
                .SetBasePath(apiProjectPath) // Указываем правильную базовую папку
                .AddJsonFile("tmp-appsettings.json")
                .AddJsonFile($"tmp-appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true)
                .Build();

            var connectionString = configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Connection string 'DefaultConnection' not found");
            }

            Database.GetDbConnection().ConnectionString = connectionString;
            Database.Migrate();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Migration failed: {ex.Message}");
            throw;
        }
    }
    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TemplateDbContext).Assembly);
    }

    protected static DbContextOptions<T> ChangeOptionsType<T>(DbContextOptions options) where T : DbContext
    {
        return new DbContextOptionsBuilder<T>()
                    .Options;
    }
}
