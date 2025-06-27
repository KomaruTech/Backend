using MediatR;
using TemplateService.Application.EventFeedback.DTOs;

namespace TemplateService.Application.EventFeedback.Queries;

public record GetEventFeedbackQuery(Guid Id) : IRequest<EventFeedbackDto>;