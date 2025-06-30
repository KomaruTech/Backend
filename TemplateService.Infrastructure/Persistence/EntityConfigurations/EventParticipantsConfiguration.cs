namespace TemplateService.Infrastructure.Persistence.EntityConfigurations;

using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


public class EventParticipantsConfiguration : IEntityTypeConfiguration<EventParticipantEntity>
{
    public void Configure(EntityTypeBuilder<EventParticipantEntity> builder)
    {
        builder.ToTable("event_participants", opts =>
        {
            opts.HasComment("Связь пользователей с мероприятиями");
        });

        // Составной ключ
        builder.HasKey(e => new { e.UserId, e.EventId });

        builder.Property(e => e.UserId)
            .HasColumnName("user_id")
            .IsRequired();
        

        builder.Property(e => e.EventId)
            .HasColumnName("event_id")
            .IsRequired();

        builder.Property(e => e.IsSpeaker)
            .HasColumnName("is_speaker")
            .IsRequired()
            .HasDefaultValue(false)
            .HasComment("Флаг, выступает ли пользователь на мероприятии");
        
        builder.Property(e => e.AttendanceResponse)
            .HasColumnName("attendance_response")
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(32)
            .HasComment("Статус, принял ли пользователь приглашение или отказал, или не ответил");
        
        builder.HasOne(e => e.User)
            .WithMany()
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.Event)
            .WithMany(ev => ev.Participants)
            .HasForeignKey(e => e.EventId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}