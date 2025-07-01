using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TemplateService.Application.Auth.Services;
using TemplateService.Application.Teams.Dtos;
using TemplateService.Application.Teams.Services;
using TemplateService.Domain.Entities;
using TemplateService.Infrastructure.Persistence;

namespace TemplateService.Application.Teams.Commands;

internal class CreateTeamCommandHandler : IRequestHandler<CreateTeamCommand, TeamsDto>
{
    private readonly TemplateDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly ITeamValidationService _teamValidationService;
    private readonly ICurrentUserService _currentUserService;

    public CreateTeamCommandHandler(
        TemplateDbContext dbContext, 
        IMapper mapper,
        ITeamValidationService teamValidationService,
        ICurrentUserService currentUserService
        )
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _teamValidationService = teamValidationService;
        _currentUserService = currentUserService;
    }

    public async Task<TeamsDto> Handle(CreateTeamCommand command, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.GetUserId();

        _teamValidationService.ValidateName(command.Name);
        _teamValidationService.ValidateDescription(command.Description);

        // Список пользователей, которых надо добавить в команду (кроме владельца)
        List<UserEntity> users = new();

        if (command.UserIds is { Count: > 0 })
        {
            // Получаем пользователей из БД
            users = await _dbContext.Users
                .Where(u => command.UserIds.Contains(u.Id))
                .ToListAsync(cancellationToken);

            // Проверка на существование всех указанных пользователей
            var foundUserIds = users.Select(u => u.Id).ToHashSet();
            var missingUserIds = command.UserIds
                .Where(id => !foundUserIds.Contains(id))
                .ToList();

            if (missingUserIds.Count > 0)
                throw new ArgumentException($"Users with IDs [{string.Join(", ", missingUserIds)}] not found.");
        }

        // Создаем команду
        var team = new TeamsEntity
        {
            Id = Guid.NewGuid(),
            Name = command.Name,
            Description = command.Description,
            OwnerId = userId
        };

        // Уникальные ID пользователей, которых надо добавить в команду (включая себя)
        var allUserIdsToAdd = users
            .Select(u => u.Id)
            .Append(userId) // Добавляем себя
            .Distinct() // На случай, если себя уже указали
            .ToList();

        team.Users = allUserIdsToAdd.Select(uid => new UserTeamsEntity
        {
            UserId = uid,
            TeamId = team.Id
        }).ToList();

        await _dbContext.Teams.AddAsync(team, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        var teamWithUsers = await _dbContext.Teams
            .Include(t => t.Users)
            .ThenInclude(ut => ut.User)
            .FirstOrDefaultAsync(t => t.Id == team.Id, cancellationToken);

        return _mapper.Map<TeamsDto>(teamWithUsers);
    }
}