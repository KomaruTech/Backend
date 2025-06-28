using TemplateService.Domain.Entities;
using TemplateService.Domain.Enums;

namespace TemplateService.Application.Event.DTOs;

public record EventDto(
    Guid Id,
    string Name,
    string Description,
    DateTime TimeStart,
    DateTime? TimeEnd,
    EventTypeEnum Type,
    string? Location,
    Guid CreatedById,
    List<string> Keywords,
    // Добавляем новые поля
    bool NotificationsEnabled = true,
    bool Reminder1DaySent = false,
    bool Reminder1HourSent = false,
    List<Guid> ParticipantIds = null // Список ID участников
);