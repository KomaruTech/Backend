using Microsoft.EntityFrameworkCore;
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

    public void Migrate()
    {
        Database.Migrate();
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
