using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using TemplateService.Application.EventFeedback.DTOs;
using TemplateService.Domain.Entities;
using TemplateService.Infrastructure.Persistence;

namespace TemplateService.Application.EventFeedback.Queries;

internal class GetEventFeedbackQueryHandler : IRequestHandler<GetEventFeedbackQuery, EventFeedbackDto>
{
    private readonly TemplateDbContext _dbContext;
    private readonly IMapper _mapper;

    public GetEventFeedbackQueryHandler(TemplateDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<EventFeedbackDto> Handle(GetEventFeedbackQuery query, CancellationToken ct)
    {
        return await _dbContext.EventFeedbacks
            .Where(u => u.Id == query.Id)
            .ProjectTo<EventFeedbackDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(ct);
    }
}