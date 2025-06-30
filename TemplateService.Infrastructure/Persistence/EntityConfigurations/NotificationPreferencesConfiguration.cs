namespace TemplateService.Infrastructure.Persistence.EntityConfigurations;

using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


public class NotificationPreferencesConfiguration: IEntityTypeConfiguration<NotificationPreferencesEntity>
{
    public void Configure(EntityTypeBuilder<NotificationPreferencesEntity> builder)
    {
        builder.ToTable("notification_preferences", opts =>
        {
            opts.HasComment("Предпочтения пользователей");
        });

        builder.HasKey(u => u.Id);
        builder.Property(u => u.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(u => u.NotifyEmail)
            .HasColumnName("notify_email")
            .HasDefaultValue(false)
            .IsRequired();
        
        builder.Property(u => u.NotifyTelegram)
            .HasColumnName("notify_telegram")
            .HasDefaultValue(false)
            .IsRequired();
        
        builder.Property(u => u.ReminderBefore1Day)
            .HasColumnName("reminder_before_1day")
            .HasDefaultValue(true)
            .IsRequired();
        
        builder.Property(u => u.ReminderBefore1Hour)
            .HasColumnName("reminder_before_1hour")
            .HasDefaultValue(true)
            .IsRequired();
    }
}