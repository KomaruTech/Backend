using MediatR;

namespace TemplateService.Application.Teams.Commands.DeleteTeam;

public record DeleteTeamCommand(Guid TeamId) : IRequest<Unit>;