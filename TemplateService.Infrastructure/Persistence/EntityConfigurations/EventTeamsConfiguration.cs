using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TemplateService.Domain.Entities;

namespace TemplateService.Infrastructure.Persistence.EntityConfigurations;

public class EventTeamsonfiguration : IEntityTypeConfiguration<EventTeamsEntity>
{
    public void Configure(EntityTypeBuilder<EventTeamsEntity> builder)
    {
        builder.ToTable("event_groups", opts =>
        {
            opts.HasComment("Связь мероприятий с группами");
        });

        builder.HasKey(x => new { x.EventId, GroupId = x.TeamId });

        builder.Property(x => x.EventId)
            .HasColumnName("event_id")
            .IsRequired();

        builder.Property(x => x.TeamId)
            .HasColumnName("group_id")
            .IsRequired();

        builder.HasOne(x => x.Event)
            .WithMany(e => e.EventTeams)
            .HasForeignKey(x => x.EventId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Team)
            .WithMany(g => g.EventTeams)
            .HasForeignKey(x => x.TeamId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}