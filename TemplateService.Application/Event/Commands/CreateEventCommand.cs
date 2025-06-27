using MediatR;
using TemplateService.Application.Event.DTOs;
using TemplateService.Domain.Entities;
using TemplateService.Domain.Enums;

namespace TemplateService.Application.Event.Commands;

public record CreateEventCommand(
    string Name,
    string Description,
    DateTime TimeStart,
    DateTime? TimeEnd,
    EventTypeEnum Type,
    string? Location,
    List<string> Keywords
) : IRequest<EventDto>;