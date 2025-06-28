using System.Text.Json;
using TemplateService.Domain.Enums;

namespace TemplateService.Infrastructure.Persistence.EntityConfigurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Entities;

public class EventConfiguration : IEntityTypeConfiguration<EventEntity>
{
    public void Configure(EntityTypeBuilder<EventEntity> builder)
    {
        builder.ToTable("events", opts => { opts.HasComment("Мероприятия"); });

        builder.HasKey(u => u.Id);
        builder.Property(u => u.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(u => u.Name)
            .HasColumnName("name")
            .IsRequired()
            .HasMaxLength(64);

        builder.Property(u => u.Description)
            .HasColumnName("description");

        builder.Property(u => u.TimeStart)
            .HasColumnName("time_start")
            .IsRequired();

        builder.Property(u => u.TimeEnd)
            .HasColumnName("time_end");

        builder.Property(e => e.Type)
            .HasColumnName("type")
            .IsRequired()
            .HasDefaultValue(EventTypeEnum.general)
            .HasConversion<string>()
            .HasMaxLength(32);

        builder.Property(u => u.Location)
            .HasColumnName("location")
            .HasMaxLength(1000);

        builder.Property(u => u.CreatedById)
            .HasColumnName("created_by_id") // имя колонки из таблицы events
            .IsRequired();


        builder.Property(e => e.Keywords)
            .HasColumnName("keywords")
            .HasColumnType("jsonb")
            .HasDefaultValueSql("'[]'::jsonb")
            .IsRequired()
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null)
            );

        builder.HasOne(e => e.CreatedBy)
            .WithMany()
            .HasForeignKey(e => e.CreatedById)
            .OnDelete(DeleteBehavior.NoAction)
            .IsRequired();

        builder.HasMany(e => e.Photos)
            .WithOne(p => p.Event)
            .HasForeignKey(p => p.EventId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasMany(e => e.Participants)
            .WithOne(p => p.Event)
            .HasForeignKey(p => p.EventId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasMany(e => e.EventTeams)
            .WithOne(eg => eg.Event)
            .HasForeignKey(eg => eg.EventId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}