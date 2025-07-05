using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using TemplateService.Application.EventFeedback.DTOs;
using TemplateService.Infrastructure.Persistence;

namespace TemplateService.Application.EventFeedback.Queries;

internal class GetAllEventFeedbacksQueryHandler : IRequestHandler<GetAllEventFeedbackQuery, List<EventFeedbackDto>>
{
    private readonly TemplateDbContext _dbContext;
    private readonly IMapper _mapper;

    public GetAllEventFeedbacksQueryHandler(TemplateDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<List<EventFeedbackDto>> Handle(GetAllEventFeedbackQuery query, CancellationToken cancellationToken)
    {
        return await _dbContext.EventFeedbacks
            .Where(f => f.EventId == query.EventId)
            .ProjectTo<EventFeedbackDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);
    }
}