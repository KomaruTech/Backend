
using MediatR;
using TemplateService.Application.Teams.Dtos;

namespace TemplateService.Application.Teams.Commands.AddTeamMember;

public record AddTeamMemberCommand(
    Guid TeamId,
    Guid UserId) : IRequest<TeamsDto>;