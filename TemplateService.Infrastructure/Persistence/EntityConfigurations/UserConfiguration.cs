using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TemplateService.Domain.Entities;

namespace TemplateService.Infrastructure.Persistence.EntityConfigurations;

public class UserConfiguration : IEntityTypeConfiguration<UserEntity>
{
    public void Configure(EntityTypeBuilder<UserEntity> builder)
    {
        builder.ToTable("users");

        builder.HasKey(u => u.Id);
        builder.Property(u => u.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(u => u.Login)
            .HasColumnName("login")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(u => u.PasswordHash)
            .HasColumnName("password_hash")
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(u => u.Name)
            .HasColumnName("name")
            .HasMaxLength(50);

        builder.Property(u => u.Surname)
            .HasColumnName("surname")
            .HasMaxLength(50);

        builder.Property(u => u.Email)
            .HasColumnName("email")
            .HasMaxLength(100);

        builder.Property(u => u.TelegramId)
            .HasColumnName("telegram_id")
            .HasMaxLength(50);

        builder.Property(u => u.NotificationPreferencesId)
            .HasColumnName("notification_preferences_id")
            .IsRequired();

        // Настройка связи с NotificationPreferences
        builder.HasOne(u => u.NotificationPreferences)
            .WithMany()
            .HasForeignKey(u => u.NotificationPreferencesId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(u => u.Login).IsUnique();
        builder.HasIndex(u => u.Email).IsUnique();
    }
}