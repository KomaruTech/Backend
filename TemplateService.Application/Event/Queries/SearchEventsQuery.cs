using System.ComponentModel.DataAnnotations;
using TemplateService.Application.Event.DTOs;
using TemplateService.Domain.Enums;

namespace TemplateService.Application.Event.Queries;

public record SearchEventsQuery(
    String? Name,
    DateTime? StartSearchTime, 
    DateTime? EndSearchTime,
    EventStatusEnum? Status,
    List<string>? Keywords
    ) : IRequest<List<EventDto>>;