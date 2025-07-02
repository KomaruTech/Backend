using TemplateService.Application.Teams.Dtos;

namespace TemplateService.Application.Teams.Queries;

public record SearchMyTeamsQuery(
    string? Name
) : IRequest<List<TeamsDto>>;