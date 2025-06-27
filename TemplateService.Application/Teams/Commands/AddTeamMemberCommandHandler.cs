
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TemplateService.Application.Teams.Commands.AddTeamMember;
using TemplateService.Application.Teams.Dtos;
using TemplateService.Domain.Entities;
using TemplateService.Infrastructure.Persistence;

namespace TemplateService.Application.Teams.Commands.AddTeamMember;

internal sealed class AddTeamMemberCommandHandler(
    TemplateDbContext dbContext,
    IMapper mapper)
    : IRequestHandler<AddTeamMemberCommand, TeamsDto>
{
    public async Task<TeamsDto> Handle(AddTeamMemberCommand request, CancellationToken ct)
    {
        var team = await dbContext.Teams
            .Include(t => t.Users)
            .FirstOrDefaultAsync(t => t.Id == request.TeamId, ct);

        var userExists = await dbContext.Users
            .AnyAsync(u => u.Id == request.UserId, ct);

        if (team == null || !userExists)
            throw new ArgumentException("Team or User not found");

        if (team.Users.Any(u => u.UserId == request.UserId))
            throw new ArgumentException("User already in team");

        //team.Users.Add(new User { UserId = request.UserId });  Здвесь ошибка, надо исправить.
        //await dbContext.SaveChangesAsync(ct);

        return mapper.Map<TeamsDto>(team);
    }
}