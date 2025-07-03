using TemplateService.Application.User.Dtos;
using TemplateService.Application.User.DTOs;

namespace TemplateService.Application.User.Queries;

public record GetNotificationPreferencesQuery() : IRequest<UserNotificationPreferencesDto>;