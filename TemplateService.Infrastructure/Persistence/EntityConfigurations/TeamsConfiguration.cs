namespace TemplateService.Infrastructure.Persistence.EntityConfigurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Entities;


public class TeamsConfiguration : IEntityTypeConfiguration<TeamsEntity>
{
    public void Configure(EntityTypeBuilder<TeamsEntity> builder)
    {
        builder.ToTable("teams", opts =>
        {
            opts.HasComment("Команды (Группы)");
        });

        builder.HasKey(u => u.Id);
        builder.Property(u => u.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(u => u.Name)
            .HasColumnName("name")
            .IsRequired()
            .HasMaxLength(255);
        
        builder.HasIndex(u => u.Name)
            .IsUnique();

        builder.Property(u => u.Description)
            .HasColumnName("description");

        builder.Property(u => u.OwnerId)
            .HasColumnName("owner_id")
            .IsRequired();
        
        builder.HasOne(u => u.Owner)
            .WithMany(o => o.CreatedTeams)
            .HasForeignKey(u => u.OwnerId)
            .OnDelete(DeleteBehavior.Cascade);

        
        builder.HasMany(u => u.Users)
            .WithOne(ut => ut.Team)
            .HasForeignKey(ut => ut.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasMany(t => t.EventTeams)
            .WithOne(eg => eg.Team)
            .HasForeignKey(eg => eg.TeamId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}