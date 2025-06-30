namespace TemplateService.Infrastructure.Persistence.EntityConfigurations;

using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class UserTeamsConfiguration : IEntityTypeConfiguration<UserTeamsEntity>
{
    public void Configure(EntityTypeBuilder<UserTeamsEntity> builder)
    {
        builder.ToTable("user_teams", opts =>
        {
            opts.HasComment("Связь пользователей и команд");
        });

        // Составной первичный ключ
        builder.HasKey(ut => new { ut.UserId, ut.TeamId });

        builder.Property(ut => ut.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        builder.Property(ut => ut.TeamId)
            .HasColumnName("team_id")
            .IsRequired();

        // Связь с UserEntity
        builder.HasOne(ut => ut.User)
            .WithMany(u => u.Teams)  // ссылка на ICollection<UserTeamsEntity>
            .HasForeignKey(ut => ut.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Связь с TeamsEntity
        builder.HasOne(ut => ut.Team)
            .WithMany(t => t.Users) // аналогично — навигационное свойство в TeamsEntity
            .HasForeignKey(ut => ut.TeamId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}