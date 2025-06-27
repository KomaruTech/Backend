using MediatR;
using TemplateService.Application.Teams.Dtos;

namespace TemplateService.Application.EventParticipant.Queries;

public record GetTeamsQuery(Guid Id) : IRequest<TeamsDto>;