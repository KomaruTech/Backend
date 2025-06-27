using MediatR;
using Microsoft.EntityFrameworkCore;
using TemplateService.Application.Teams.Commands.DeleteTeam;
using TemplateService.Domain.Entities;
using TemplateService.Infrastructure.Persistence;

namespace TemplateService.Application.Teams.Commands.DeleteTeam;

internal sealed class DeleteTeamCommandHandler(
    TemplateDbContext dbContext)
    : IRequestHandler<DeleteTeamCommand, Unit>
{
    public async Task<Unit> Handle(DeleteTeamCommand request, CancellationToken ct)
    {
        var team = await dbContext.Teams
            .FirstOrDefaultAsync(t => t.Id == request.TeamId, ct);

        if (team != null)
        {
            dbContext.Teams.Remove(team);
            await dbContext.SaveChangesAsync(ct);
        }

        return Unit.Value;
    }
}