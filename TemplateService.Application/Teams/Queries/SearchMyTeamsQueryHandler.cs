using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using TemplateService.Application.Auth.Services;
using TemplateService.Application.Teams.Dtos;
using TemplateService.Infrastructure.Persistence;

namespace TemplateService.Application.Teams.Queries;

internal class SearchMyTeamsQueryHandler : IRequestHandler<SearchTeamsQuery, List<TeamsDto>>
{
    private readonly TemplateDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;

    public SearchMyTeamsQueryHandler(
        TemplateDbContext dbContext,
        IMapper mapper,
        ICurrentUserService currentUserService
    )
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _currentUserService = currentUserService;
    }

    public async Task<List<TeamsDto>> Handle(SearchTeamsQuery query, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.GetUserId();

        var teamsQuery = _dbContext.Teams
            .Include(t => t.Users)
            .ThenInclude(ut => ut.User)
            .Where(t => t.Users.Any(ut => ut.UserId == userId))
            .AsQueryable();

        if (string.IsNullOrWhiteSpace(query.Name))
        {
            return await teamsQuery
                .OrderBy(t => EF.Functions.Random())
                .ProjectTo<TeamsDto>(_mapper.ConfigurationProvider)
                .Take(100)
                .ToListAsync(cancellationToken);
        }

        var q = query.Name;

        teamsQuery = teamsQuery
            .Where(t => EF.Functions.ILike(t.Name, $"%{q}%"))
            .OrderByDescending(t =>
                EF.Functions.ILike(t.Name, q) ? 100 :
                EF.Functions.ILike(t.Name, $"%{q}%") ? 60 :
                0
            )
            .ThenBy(t => t.Name)
            .Take(100);

        return await teamsQuery
            .ProjectTo<TeamsDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);
    }
}