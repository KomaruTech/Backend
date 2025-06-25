using Microsoft.Data.SqlClient; 
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Npgsql; 
using TemplateService.Domain.Entities;

namespace TemplateService.Infrastructure.Persistence;

public class TemplateDbContext : DbContext
{
    protected readonly string _defaultSchema = "XXATACH_TMP";

    public TemplateDbContext(DbContextOptions<TemplateDbContext> options)
        : base(options)
    {
    }

    public DbSet<DocumentEntity> Documents { get; set; }
    public DbSet<MetaEntity> Metas { get; set; }
    public DbSet<UserEntity> Users { get; set; }

    public void Migrate()
    {
        try
        {
            // Получаем конфигурацию из DI
            var serviceProvider = this.GetService<IServiceProvider>();
            var configuration = serviceProvider.GetRequiredService<IConfiguration>();

            var connectionString = configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Connection string 'DefaultConnection' not found");
            }

            // Применяем миграции
            this.Database.Migrate();
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
        modelBuilder.HasDefaultSchema(_defaultSchema);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TemplateDbContext).Assembly);
    }

    protected static DbContextOptions<T> ChangeOptionsType<T>(DbContextOptions options) where T : DbContext
    {
        return new DbContextOptionsBuilder<T>()
                    .Options;
    }
}
