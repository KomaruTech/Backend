using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using TemplateService.Application.Auth.Services;
using TemplateService.Application.EventParticipant.Queries;
using TemplateService.Application.SpeakerApplication.Dtos;
using TemplateService.Application.Teams.Dtos;
using TemplateService.Domain.Entities;
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
            .Where(t => t.Users.Any(ut => ut.UserId == userId))
            .AsQueryable();

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