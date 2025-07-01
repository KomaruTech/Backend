using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using TemplateService.Application.NotificationPreferences.Dtos;
using TemplateService.Application.NotificationPreferences.Queries;
using TemplateService.Domain.Entities;
using TemplateService.Infrastructure.Persistence;

namespace TemplateService.Application.NotificationPreferences.Queries;

internal class GetNotificationPreferencesQueryHandler : IRequestHandler<GetNotificationPreferencesQuery, NotificationPreferencesDto>
{
    private readonly TemplateDbContext _dbContext;
    private readonly IMapper _mapper;

    public GetNotificationPreferencesQueryHandler(TemplateDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<NotificationPreferencesDto> Handle(GetNotificationPreferencesQuery query, CancellationToken ct)
    {
        return await _dbContext.EventFeedbacks
            .Where(u => u.Id == query.Id)
            .ProjectTo<NotificationPreferencesDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(ct);
    }
}