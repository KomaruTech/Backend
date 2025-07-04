using Microsoft.EntityFrameworkCore;
using TemplateService.Domain.Entities;

namespace TemplateService.Infrastructure.Persistence;

public class TemplateDbContext : DbContext
{
    protected readonly string _defaultSchema = "DEFAULT";

    public TemplateDbContext(DbContextOptions<TemplateDbContext> options)
        : base(options)
    {
    }

    // DbSet'ы сущностей
    public DbSet<UserEntity> Users { get; set; }
    public DbSet<TeamsEntity> Teams { get; set; }
    public DbSet<UserTeamsEntity> UserTeams { get; set; }
    public DbSet<NotificationPreferencesEntity> NotificationPreferences { get; set; }
    public DbSet<EventEntity> Events { get; set; }
    public DbSet<EventFeedbackEntity> EventFeedbacks { get; set; }
    public DbSet<EventParticipantEntity> EventParticipants { get; set; }
    public DbSet<SpeakerApplicationEntity> SpeakerApplications { get; set; }
    public DbSet<EventNotificationsEntity> EventNotifications { get; set; }
    public DbSet<EventTeamsEntity> EventTeams { get; set; }

    public void Migrate()
    {
        Database.Migrate();
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
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
