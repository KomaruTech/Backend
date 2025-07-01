namespace TemplateService.Infrastructure.Persistence.EntityConfigurations;

using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


public class EventFeedbackConfiguration : IEntityTypeConfiguration<EventFeedbackEntity>
{
    public void Configure(EntityTypeBuilder<EventFeedbackEntity> builder)
    {
        builder.ToTable("event_feedback", opts =>
        {
            opts.HasComment("Отзывы на мероприятия");
            opts.HasCheckConstraint("CK_event_feedback_rating_range", "rating BETWEEN 1 AND 5");
        });

        builder.HasKey(f => f.Id);

        builder.Property(f => f.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(f => f.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        builder.Property(f => f.EventId)
            .HasColumnName("event_id")
            .IsRequired();

        builder.Property(f => f.Rating)
            .HasColumnName("rating")
            .IsRequired()
            .HasColumnType("smallint");

        builder.Property(f => f.CreatedAt)
            .HasColumnName("created_at")
            .HasDefaultValueSql("now()")
            .IsRequired();

        builder.Property(f => f.Comment)
            .HasColumnName("comment");

        builder.HasOne(f => f.User)
            .WithMany()
            .HasForeignKey(f => f.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(f => f.Event)
            .WithMany()
            .HasForeignKey(f => f.EventId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}