using MediatR;
using TemplateService.Application.EventFeedback.DTOs;

namespace TemplateService.Application.EventFeedback.Queries;

public record GetUserEventFeedbackQuery(Guid UserId) : IRequest<List<EventFeedbackDto>>;