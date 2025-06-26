using MediatR;
using TemplateService.Application.UserTeams.Dtos;

namespace TemplateService.Application.UserTeams.Queries;

public record GetUserTeamsQuery(Guid Id) : IRequest<UserTeamsDto>;

