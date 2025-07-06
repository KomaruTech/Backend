namespace TemplateService.Application.Teams.Commands;

public record DeleteTeamCommand(Guid TeamId) : IRequest<Unit>;