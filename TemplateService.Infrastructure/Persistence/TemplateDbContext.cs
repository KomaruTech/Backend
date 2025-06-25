
﻿using Microsoft.Data.SqlClient; 
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Npgsql; 

﻿using Microsoft.EntityFrameworkCore;

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


    protected override void OnConfiguring(DbContextOptionsBuilder options) { 

        Database.Migrate();
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {

    }

    // Подключение конфигураций сущностей
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema(_defaultSchema);

        
        modelBuilder.HasDefaultSchema(_defaultSchema);
        

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TemplateDbContext).Assembly);
    }

    protected static DbContextOptions<T> ChangeOptionsType<T>(DbContextOptions options) where T : DbContext
    {
        return new DbContextOptionsBuilder<T>()
                    .Options;
    }
}
