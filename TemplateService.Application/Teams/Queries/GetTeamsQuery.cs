using MediatR;
using TemplateService.Application.Teams.Dtos;

namespace TemplateService.Application.Teams.Queries;

public record GetTeamsQuery(Guid Id) : IRequest<TeamsDto>;