using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TemplateService.Application.Event.DTOs;
using TemplateService.Infrastructure.Persistence;

namespace TemplateService.Application.Event.Queries;

public class SearchEventsHandler : IRequestHandler<SearchInIntervalQuery, List<EventDto>>
{
    private readonly TemplateDbContext _dbContext;
    private readonly IMapper _mapper;

    public SearchEventsHandler(TemplateDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<List<EventDto>> Handle(SearchInIntervalQuery request, CancellationToken cancellationToken)
    {
        var query = _dbContext.Events.AsQueryable();

        if (request.StartDate.HasValue)
        {
            query = query.Where(e => e.TimeStart >= request.StartDate.Value);
        }

        if (request.EndDate.HasValue)
        {
            query = query.Where(e => !e.TimeEnd.HasValue || e.TimeEnd <= request.EndDate.Value);
        }

        var events = await query.ToListAsync(cancellationToken);
        return _mapper.Map<List<EventDto>>(events);
    }
}