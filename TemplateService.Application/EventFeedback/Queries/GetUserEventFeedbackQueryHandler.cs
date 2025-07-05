using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using TemplateService.Application.EventFeedback.DTOs;
using TemplateService.Infrastructure.Persistence;

namespace TemplateService.Application.EventFeedback.Queries;

internal class GetUserEventFeedbacksQueryHandler : IRequestHandler<GetUserEventFeedbackQuery, List<EventFeedbackDto>>
{
    private readonly TemplateDbContext _dbContext;
    private readonly IMapper _mapper;

    public GetUserEventFeedbacksQueryHandler(TemplateDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<List<EventFeedbackDto>> Handle(GetUserEventFeedbackQuery query, CancellationToken cancellationToken)
    {
        return await _dbContext.EventFeedbacks
            .Where(f => f.UserId == query.UserId)
            .ProjectTo<EventFeedbackDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);
    }
}