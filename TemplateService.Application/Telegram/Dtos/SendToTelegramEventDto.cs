using TemplateService.Domain.Enums;

namespace TemplateService.Application.TelegramService;

public record SendToTelegramEventDto
(
    string Name,
    string Description,
    DateTime TimeStart,
    DateTime? TimeEnd,
    EventTypeEnum Type,
    string? Location,
    long TelegramUserId
);