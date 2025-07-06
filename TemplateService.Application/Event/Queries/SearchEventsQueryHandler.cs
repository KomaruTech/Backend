using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TemplateService.Application.Auth.Services;
using TemplateService.Application.Event.DTOs;
using TemplateService.Domain.Entities;
using TemplateService.Domain.Enums;
using TemplateService.Infrastructure.Persistence;

namespace TemplateService.Application.Event.Queries;

public class SearchEventsHandler : IRequestHandler<SearchEventsQuery, List<EventDto>>
{
    private readonly TemplateDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;

    public SearchEventsHandler(
        TemplateDbContext dbContext,
        IMapper mapper,
        ICurrentUserService currentUserService
    )
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _currentUserService = currentUserService;
    }

    public async Task<List<EventDto>> Handle(SearchEventsQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.GetUserId();
        
        var userTeamIds = await _dbContext.UserTeams
            .Where(ut => ut.UserId == userId)
            .Select(ut => ut.TeamId)
            .ToListAsync(cancellationToken);
        
        var query = _dbContext.Events
            .Include(e => e.Participants)
            .Include(e => e.EventTeams)
            .AsQueryable();
        
        if (request.StartSearchTime.HasValue)
            query = query.Where(e => e.TimeStart >= request.StartSearchTime);
        
        if (request.EndSearchTime.HasValue)
            query = query.Where(e => !e.TimeEnd.HasValue || e.TimeEnd <= request.EndSearchTime.Value);
        
        if (request.Status != null)
            query = query.Where(e => e.Status == request.Status);
        
        if (!string.IsNullOrWhiteSpace(request.Name))
            query = query.Where(e => EF.Functions.ILike(e.Name, $"%{request.Name}%"));
        
        query = query.Where(e =>
            e.Type == EventTypeEnum.general ||
            (e.Type == EventTypeEnum.personal && e.Participants.Any(p => p.UserId == userId)) ||
            (e.Type == EventTypeEnum.group && e.EventTeams.Any(eg => userTeamIds.Contains(eg.TeamId)))
        );
        
        List<EventEntity> events;

        if (request.Keywords != null && request.Keywords.Any())
        {
            var filterKeywordsLower = request.Keywords.Select(k => k.ToLower()).ToList();

            events = await query.ToListAsync(cancellationToken);

            events = events
                .Where(e => e.Keywords.Any(kw => filterKeywordsLower.Contains(kw.ToLower())))
                .ToList();
        }
        else
        {
            events = await query.ToListAsync(cancellationToken);
        }
        
        return _mapper.Map<List<EventDto>>(events);
    }
}