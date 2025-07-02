using TemplateService.Application.Teams.Dtos;

namespace TemplateService.Application.Teams.Commands;

using MediatR;

public record DeleteTeamMemberCommand(
    Guid TeamId,
    Guid UserId
    ) : IRequest<TeamsDto>;