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
        
        var query = _dbContext.Events
            .Include(e => e.Participants) // обязательно для фильтра по участникам
            .AsQueryable();
        
        query = query.Where(e => e.TimeStart >= request.StartSearchTime);

        if (request.EndSearchTime.HasValue)
            query = query.Where(e => !e.TimeEnd.HasValue || e.TimeEnd <= request.EndSearchTime.Value);
       
        // ID групп пользователя
        var userGroupIds = await _dbContext.UserTeams
            .Where(ug => ug.UserId == userId)
            .Select(ug => ug.TeamId)
            .ToListAsync(cancellationToken);
        
        // Фильтруем по типам, какие мероприятия пользователь должен видеть
        query = query.Where(e =>
                e.Type == EventTypeEnum.general || // Все видят
                (e.Type == EventTypeEnum.personal && e.Participants.Any(p => p.UserId == userId)) || // Видят только выбранные
                (e.Type == EventTypeEnum.group && e.EventTeams.Any(eg => userGroupIds.Contains(eg.TeamId))) // Видит вся группа
        );
        
        List<EventEntity> events;

        if (request.Keywords != null && request.Keywords.Any())
        {
            var filterKeywordsLower = request.Keywords.Select(k => k.ToLower()).ToList();

            events = query
                .AsEnumerable() // переключение на LINQ to Objects
                .Where(e =>
                    e.Keywords.Any(kw => filterKeywordsLower.Contains(kw.ToLower()))
                )
                .ToList();
        }
        else
        {
            events = await query.ToListAsync(cancellationToken);
        }

        return _mapper.Map<List<EventDto>>(events);
    }
}