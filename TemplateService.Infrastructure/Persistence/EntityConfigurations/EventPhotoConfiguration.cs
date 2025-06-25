namespace TemplateService.Infrastructure.Persistence.EntityConfigurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Entities;

public class EventPhotoConfiguration : IEntityTypeConfiguration<EventPhotoEntity>
{
    public void Configure(EntityTypeBuilder<EventPhotoEntity> builder)
    {
        builder.ToTable("event_photos", opts =>
        {
            opts.HasComment("Фотографии, прикреплённые к событиям");
        });

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(p => p.EventId)
            .HasColumnName("event_id")
            .IsRequired();

        builder.Property(p => p.Image)
            .HasColumnName("image")
            .IsRequired()
            .HasColumnType("bytea");

        builder.Property(p => p.MimeType)
            .HasColumnName("mime_type")
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(p => p.UploadedAt)
            .HasColumnName("uploaded_at")
            .HasDefaultValueSql("now()")
            .IsRequired();

        builder.Property(p => p.Description)
            .HasColumnName("description");

        builder.HasOne(p => p.Event)
            .WithMany(e => e.Photos)
            .HasForeignKey(p => p.EventId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();
    }
}