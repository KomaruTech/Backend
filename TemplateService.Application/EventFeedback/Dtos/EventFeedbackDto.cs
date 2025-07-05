using TemplateService.Domain.Entities;
using TemplateService.Domain.Enums;

namespace TemplateService.Application.EventFeedback.DTOs;

public record EventFeedbackDto(
    Guid Id,  
    Guid UserId,
    Guid EventId,
    short Rating,
    DateTime CreatedAt,
    string? Comment );