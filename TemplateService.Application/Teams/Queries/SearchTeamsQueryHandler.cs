using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using TemplateService.Application.Teams.Dtos;
using TemplateService.Infrastructure.Persistence;

namespace TemplateService.Application.Teams.Queries;

internal class SearchTeamsQueryHandler : IRequestHandler<SearchTeamsQuery, List<TeamsDto>>
{
    private readonly TemplateDbContext _dbContext;
    private readonly IMapper _mapper;

    public SearchTeamsQueryHandler(TemplateDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<List<TeamsDto>> Handle(SearchTeamsQuery query, CancellationToken cancellationToken)
    {
        var teamsQuery = _dbContext.Teams.AsQueryable();

        if (string.IsNullOrWhiteSpace(query.Name))
            return await teamsQuery
                .OrderBy(t => EF.Functions.Random())
                .ProjectTo<TeamsDto>(_mapper.ConfigurationProvider)
                .Take(100)
                .ToListAsync(cancellationToken);


        var q = query.Name;

        teamsQuery = teamsQuery
            .Where(t => EF.Functions.ILike(t.Name, $"%{q}%")) // фильтр по имени (содержит)
            .OrderByDescending(t =>
                EF.Functions.ILike(t.Name, q) ? 100 : // точное совпадение имени
                EF.Functions.ILike(t.Name, $"%{q}%") ? 60 : // имя содержит q
                0
            )
            .ThenBy(t => t.Name)
            .Take(100);

        return await teamsQuery
            .ProjectTo<TeamsDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);
    }
}