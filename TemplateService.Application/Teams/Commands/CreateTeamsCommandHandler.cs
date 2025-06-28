using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TemplateService.Application.Teams.Commands;
using TemplateService.Application.Teams.Dtos;
using TemplateService.Domain.Entities;
using TemplateService.Infrastructure.Persistence;

namespace TemplateService.Application.Teams.Commands;

internal class CreateTeamsCommandHandler : IRequestHandler<CreateTeamsCommand, TeamsDto>
{
    private readonly TemplateDbContext _dbContext;
    private readonly IMapper _mapper;

    public CreateTeamsCommandHandler(TemplateDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<TeamsDto> Handle(CreateTeamsCommand command, CancellationToken ct)
    {
        // Получаем пользователей из БД
        var users = await _dbContext.Users
            .Where(u => command.UserIds.Contains(u.Id))
            .ToListAsync(ct);

        // Создаем новую команду
        var team = new TeamsEntity
        {
            Id = Guid.NewGuid(),
            Name = command.Name,
            Description = command.Description
        };

        // Добавляем связи с пользователями
        team.Users = users.Select(u => new UserTeamsEntity
        {
            UserId = u.Id,
            TeamId = team.Id // Теперь team уже существует
        }).ToList();

        _dbContext.Teams.Add(team);
        await _dbContext.SaveChangesAsync(ct);

        return _mapper.Map<TeamsDto>(team);
    }
}