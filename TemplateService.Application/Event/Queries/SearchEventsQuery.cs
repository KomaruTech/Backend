using System.ComponentModel.DataAnnotations;
using TemplateService.Application.Event.DTOs;

namespace TemplateService.Application.Event.Queries;

public record SearchEventsQuery(
    DateTime StartSearchTime, 
    DateTime? EndSearchTime,
    List<string>? Keywords
    ) : IRequest<List<EventDto>>;