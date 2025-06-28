using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using TemplateService.Application.Event.DTOs;
using TemplateService.Domain.Enums;
using TemplateService.Infrastructure.Persistence;

namespace TemplateService.Application.Event.Queries;

public class SearchEventsHandler : IRequestHandler<SearchInIntervalQuery, List<EventDto>>
{
    private readonly TemplateDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public SearchEventsHandler(
        TemplateDbContext dbContext,
        IMapper mapper,
        IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<List<EventDto>> Handle(SearchInIntervalQuery request, CancellationToken cancellationToken)
    {
        var jwtUser = _httpContextAccessor.HttpContext?.User;

        var idClaim = jwtUser.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        if (!Guid.TryParse(idClaim, out var userId))
            throw new UnauthorizedAccessException("Invalid user ID");

        var query = _dbContext.Events
            .Include(e => e.Participants) // обязательно для фильтра по участникам
            .AsQueryable();

        if (request.StartDate.HasValue)
            query = query.Where(e => e.TimeStart >= request.StartDate.Value);

        if (request.EndDate.HasValue)
            query = query.Where(e => !e.TimeEnd.HasValue || e.TimeEnd <= request.EndDate.Value);

        // ID групп пользователя
        var userGroupIds = await _dbContext.UserTeams
            .Where(ug => ug.UserId == userId)
            .Select(ug => ug.TeamId)
            .ToListAsync(cancellationToken);

        query = query.Where(e =>
                e.Type == EventTypeEnum.general || // Все видят
                (e.Type == EventTypeEnum.personal && e.Participants.Any(p => p.UserId == userId)) || // Видят только выбранные
                (e.Type == EventTypeEnum.group && e.EventTeams.Any(eg => userGroupIds.Contains(eg.TeamId))) // Видит вся группа
        );

        var events = await query.ToListAsync(cancellationToken);
        return _mapper.Map<List<EventDto>>(events);
    }
}