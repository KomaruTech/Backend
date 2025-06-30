using TemplateService.Application.Teams.Dtos;

namespace TemplateService.Application.Teams.Commands;

public record CreateTeamCommand(
    string Name,
    string Description,
    ICollection<Guid>? UserIds) : IRequest<TeamsDto>;