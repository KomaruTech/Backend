namespace TemplateService.Infrastructure.Persistence.EntityConfigurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TemplateService.Domain.Entities;

public class EventNotificationEntityConfiguration : IEntityTypeConfiguration<EventNotificationsEntity>
{
    public void Configure(EntityTypeBuilder<EventNotificationsEntity> builder)
    {
        // Таблица
        builder.ToTable("event_notifications");
        
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        builder.Property(e => e.UserId)
            .IsRequired();

        builder.Property(e => e.EventId)
            .IsRequired();

        builder.Property(e => e.NotificationType)
            .HasConversion<string>()
            .HasMaxLength(10)
            .IsRequired();

        builder.HasOne(e => e.User)
            .WithMany() // Или с коллекцией уведомлений, если есть
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.Event)
            .WithMany() // Или с коллекцией уведомлений, если есть
            .HasForeignKey(e => e.EventId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasIndex(e => new { e.UserId, e.EventId, e.NotificationType })
            .IsUnique(false);
    }
}