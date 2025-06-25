using TemplateService.Domain.Enums;

namespace TemplateService.Infrastructure.Persistence.EntityConfigurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Entities;

public class SpeakerApplicationsConfiguration : IEntityTypeConfiguration<SpeakerApplicationEntity>
{
    public void Configure(EntityTypeBuilder<SpeakerApplicationEntity> builder)
    {
        builder.ToTable("speaker_applications", opts =>
        {
            opts.HasComment("Заявки на выступление");
        });

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(e => e.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        builder.Property(e => e.EventId)
            .HasColumnName("event_id")
            .IsRequired();

        builder.Property(e => e.Topic)
            .HasColumnName("topic")
            .IsRequired();

        builder.Property(e => e.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired()
            .HasDefaultValueSql("now()");

        builder.Property(e => e.Comment)
            .HasColumnName("comment");

        builder.Property(e => e.Status)
            .HasColumnName("status")
            .HasColumnType("application_status")
            .IsRequired()
            .HasDefaultValue(ApplicationStatusEnum.Pending);

        builder.HasOne(e => e.User)
            .WithMany()
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.Event)
            .WithMany()
            .HasForeignKey(e => e.EventId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}