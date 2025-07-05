using MediatR;
using TemplateService.Application.EventFeedback.DTOs;

namespace TemplateService.Application.EventFeedback.Queries;

public record GetAllEventFeedbackQuery(Guid EventId) : IRequest<List<EventFeedbackDto>>;