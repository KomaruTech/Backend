#nullable enable
using TemplateService.Domain.Enums;

namespace TemplateService.Application.Event.DTOs;

public record EventDto(
    Guid Id,
    string Name, // Изменено с Name на Title
    string Description,
    DateTime TimeStart,
    DateTime? TimeEnd,
    EventTypeEnum Type,
    string? Location,
    Guid CreatedById,
    List<string> Keywords,
    bool NotificationsEnabled = true,
    bool Reminder1DaySent = false,
    bool Reminder1HourSent = false,
    List<Guid> ParticipantIds = null
);