using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Npgsql;
using TemplateService.Domain.Entities;
using Microsoft.Extensions.DependencyInjection; // Добавьте это

namespace TemplateService.Infrastructure.Persistence;

public class TemplateDbContext : DbContext
{
    
    protected readonly string _defaultSchema = "XXATACH_TMP";
    protected IConfiguration Configuration { get; } // Делаем protected property вместо field

    public TemplateDbContext(
        DbContextOptions options, // Изменяем на не-generic версию
        IConfiguration configuration) : base(options)
    {
        Configuration = configuration;
    }


    public DbSet<UserEntity> Users { get; set; }

    public void Migrate()
    {
        try
        {
            var connectionString = Configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Connection string 'DefaultConnection' not found");
            }

            Database.Migrate();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Migration failed: {ex.Message}");
            throw;
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(_defaultSchema);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TemplateDbContext).Assembly);
    }
}