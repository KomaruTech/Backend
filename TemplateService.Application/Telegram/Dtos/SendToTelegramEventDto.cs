using TemplateService.Domain.Enums;

namespace TemplateService.Application.TelegramService;

public record SendToTelegramEventDto
(
    Guid EventId,
    string Name,
    string Description,
    DateTime TimeStart,
    DateTime? TimeEnd,
    EventTypeEnum Type,
    string? Location,
    long TelegramUserId
);