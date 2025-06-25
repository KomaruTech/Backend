using MediatR;
using TemplateService.Application.Event.DTOs;

namespace TemplateService.Application.Event.Queries;

public record GetEventQuery(Guid Id) : IRequest<EventDto>;