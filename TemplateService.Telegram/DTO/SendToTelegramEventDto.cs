using TemplateService.Domain.Enums;

namespace TemplateService.Telegram.DTO;

public record SendToTelegramEventDto
(
    string Name,
    string Description,
    DateTime TimeStart,
    DateTime? TimeEnd,
    EventTypeEnum Type,
    string Location,
    long TelegramUserId
);