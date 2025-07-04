﻿using AutoMapper;
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
        
        if (request.StartSearchTime.HasValue)
            query = query.Where(e => e.TimeStart >= request.StartSearchTime);

        if (request.EndSearchTime.HasValue)
            query = query.Where(e => !e.TimeEnd.HasValue || e.TimeEnd <= request.EndSearchTime.Value);
       
        // Фильтрация по определенному статусу
        if (request.Status != null)
            query = query.Where(e => e.Status == request.Status);
        
        // Фильтрация по имени (вхождение подстроки, игнор регистра)
        if (!string.IsNullOrWhiteSpace(request.Name))
            query = query.Where(e => EF.Functions.ILike(e.Name, $"%{request.Name}%"));
        
        // Фильтруем по типам, какие мероприятия пользователь должен видеть
        query = query.Where(e =>
                e.Type == EventTypeEnum.general || // Все видят
                (e.Type == EventTypeEnum.personal && e.Participants.Any(p => p.UserId == userId)) || // Только участники
                (e.Type == EventTypeEnum.group && e.EventTeams.Any(eg =>
                    _dbContext.UserTeams.Any(ut => ut.UserId == userId && ut.TeamId == eg.TeamId))) // Участник группы
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