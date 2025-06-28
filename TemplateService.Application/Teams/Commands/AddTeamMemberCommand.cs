
using MediatR;
using TemplateService.Application.Teams.Dtos;

namespace TemplateService.Application.Teams.Commands;

public record AddTeamMemberCommand(
    Guid TeamId,
    Guid UserId) : IRequest<TeamsDto>;