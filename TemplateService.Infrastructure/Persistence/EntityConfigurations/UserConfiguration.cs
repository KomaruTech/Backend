using TemplateService.Domain.Enums;

namespace TemplateService.Infrastructure.Persistence.EntityConfigurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Entities;


public class UserConfiguration : IEntityTypeConfiguration<UserEntity>
{
    public void Configure(EntityTypeBuilder<UserEntity> builder)
    {
        builder.ToTable("users", opts =>
        {
            opts.HasComment("Пользователи");
        });

        builder.HasKey(u => u.Id);
        builder.Property(u => u.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(u => u.Login)
            .HasColumnName("login")
            .IsRequired()
            .HasMaxLength(32);

        builder.HasIndex(u => u.Login)
            .IsUnique();

        builder.Property(u => u.PasswordHash)
            .HasColumnName("password_hash")
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(u => u.Name)
            .HasColumnName("name")
            .IsRequired()
            .HasMaxLength(32);

        builder.Property(u => u.Surname)
            .HasColumnName("surname")
            .IsRequired()
            .HasMaxLength(64);

        builder.Property(e => e.Role)
            .HasColumnName("role")
            .IsRequired()
            .HasDefaultValue(UserRoleEnum.member)
            .HasConversion<string>()
            .HasMaxLength(32);
        
        builder.HasIndex(u => u.Role);

        builder.Property(u => u.Email)
            .HasColumnName("email")
            .HasMaxLength(64);

        builder.Property(u => u.TelegramId)
            .HasColumnName("telegram_id")
            .HasColumnType("bigint")
            .IsRequired(false); // разрешить null;
        
        builder.HasIndex(u => u.TelegramId)
            .IsUnique();
        
        builder.Property(u => u.TelegramUsername)
            .HasColumnName("telegram_username")
            .HasMaxLength(32);
        
        builder.HasIndex(u => u.TelegramUsername)
            .IsUnique();

        builder.Property(u => u.NotificationPreferencesId)
            .HasColumnName("notification_preferences_id")
            .IsRequired();

        builder.Property(u => u.Avatar)
            .HasColumnName("avatar")
            .HasColumnType("bytea");
        
        builder.Property(p => p.AvatarMimeType)
            .HasColumnName("avatar_mime_type")
            .HasMaxLength(255);

        builder.HasOne(u => u.NotificationPreferences)
            .WithOne()
            .HasForeignKey<UserEntity>(u => u.NotificationPreferencesId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();
        
        builder.HasMany(u => u.Teams)
            .WithOne(ut => ut.User)
            .HasForeignKey(ut => ut.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}