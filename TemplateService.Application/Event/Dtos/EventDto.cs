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
    List<string> Keywords
);