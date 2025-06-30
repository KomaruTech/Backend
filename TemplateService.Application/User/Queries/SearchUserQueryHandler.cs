using TemplateService.Application.User.DTOs;
using TemplateService.Infrastructure.Persistence;
using AutoMapper;
using Microsoft.EntityFrameworkCore;


namespace TemplateService.Application.User.Queries;

internal class SearchUserQueryHandler : IRequestHandler<SearchUserQuery, List<UserDto>>
{
    private readonly TemplateDbContext _dbContext;
    private readonly IMapper _mapper;

    public SearchUserQueryHandler(
        TemplateDbContext dbContext,
        IMapper mapper
    )
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<List<UserDto>> Handle(SearchUserQuery query, CancellationToken cancellationToken)
    {
        Console.WriteLine(query.Query);
        var usersQuery = _dbContext.Users.AsQueryable();

        // Фильтр по роли, если указан
        if (query.Role.HasValue)
            usersQuery = usersQuery.Where(u => u.Role == query.Role.Value);

        // Поиск по Query (Name и Surname)
        if (!string.IsNullOrWhiteSpace(query.Query))
        {
            var q = query.Query.Trim();

            var filteredQuery = usersQuery
                // Фильтруем пользователей, у которых имя или фамилия или их комбинация содержит подстроку q (без учета регистра)
                .Where(u =>
                    EF.Functions.ILike(u.Name, $"%{q}%") ||
                    EF.Functions.ILike(u.Surname, $"%{q}%") ||
                    EF.Functions.ILike(u.Name + " " + u.Surname, $"%{q}%") ||
                    EF.Functions.ILike(u.Surname + " " + u.Name, $"%{q}%")
                )
                // Сортируем по релевантности, вычисляемой сразу в ORDER BY, без отдельного Select
                .OrderByDescending(u =>
                        EF.Functions.ILike(u.Name + " " + u.Surname, q) ? 100 : // Имя + фамилия ровно совпадает с q
                        EF.Functions.ILike(u.Surname + " " + u.Name, q) ? 90 : // Фамилия + имя ровно совпадает с q
                        EF.Functions.ILike(u.Surname, q) ? 80 : // Фамилия ровно совпадает с q
                        EF.Functions.ILike(u.Name, q) ? 70 : // Имя ровно совпадает с q
                        EF.Functions.ILike(u.Name, $"%{q}%") ? 60 : // Имя содержит q
                        EF.Functions.ILike(u.Surname, $"%{q}%") ? 60 : // Фамилия содержит q
                        EF.Functions.ILike(u.Name + " " + u.Surname, $"%{q}%") ? 40 : // Имя + фамилия содержит q
                        EF.Functions.ILike(u.Surname + " " + u.Name, $"%{q}%") ? 30 : // Фамилия + имя содержит q
                        0 // Нет совпадений
                )
                .ThenBy(u => u.Surname)
                .ThenBy(u => u.Name).Take(100);

            var result = await filteredQuery
                .ToListAsync(cancellationToken);

            return _mapper.Map<List<UserDto>>(result);
        }

        // Если нет запроса для поиска, вернуть всех пользователей по роли или всех
        var allUsers = await usersQuery
            .OrderBy(u => u.Surname)
            .ThenBy(u => u.Name)
            .Take(100)
            .ToListAsync(cancellationToken);

        return _mapper.Map<List<UserDto>>(allUsers);
    }
}