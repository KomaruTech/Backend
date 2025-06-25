using TemplateService.Domain.Enums;

namespace TemplateService.Infrastructure.Persistence.EntityConfigurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Entities;


public class EventConfiguration : IEntityTypeConfiguration<EventEntity>
{
    public void Configure(EntityTypeBuilder<EventEntity> builder)
    {
        builder.ToTable("events", opts =>
        {
            opts.HasComment("Мероприятия");
        });

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
            .HasColumnType("event_type")
            .IsRequired()
            .HasDefaultValue(EventTypeEnum.General);
        
        builder.Property(u => u.Type)
            .HasColumnName("type")
            .IsRequired();
            

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
            .IsRequired();

        builder.HasOne(e => e.CreatedBy)
            .WithMany()
            .HasForeignKey(e => e.CreatedById)
            .OnDelete(DeleteBehavior.NoAction)
            .IsRequired();
        
        builder.HasMany(e => e.Photos)
            .WithOne(p => p.Event)
            .HasForeignKey(p => p.EventId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}