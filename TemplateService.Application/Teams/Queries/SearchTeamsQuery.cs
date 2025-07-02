using TemplateService.Application.Teams.Dtos;

namespace TemplateService.Application.Teams.Queries;

public record SearchTeamsQuery(string? Name) : IRequest<List<TeamsDto>>;