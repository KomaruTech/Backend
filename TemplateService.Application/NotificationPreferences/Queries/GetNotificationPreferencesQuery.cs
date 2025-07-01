using MediatR;
using TemplateService.Application.NotificationPreferences.Dtos;

namespace TemplateService.Application.NotificationPreferences.Queries;

public record GetNotificationPreferencesQuery(Guid Id) : IRequest<NotificationPreferencesDto>;