using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using TemplateService.Application.Auth.Services;
using TemplateService.Application.Event.DTOs;
using TemplateService.Application.Event.Services;
using TemplateService.Domain.Enums;
using TemplateService.Infrastructure.Persistence;

namespace TemplateService.Application.Event.Queries;

internal class InvitedEventsQueryHandler : IRequestHandler<InvitedEventsQuery, List<EventDto>>
{
    private readonly TemplateDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;

    public InvitedEventsQueryHandler(
        TemplateDbContext dbContext,
        IMapper mapper,
        ICurrentUserService currentUserService
    )
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _currentUserService = currentUserService;
    }

    public async Task<List<EventDto>> Handle(InvitedEventsQuery query, CancellationToken ct)
    {
        var userId = _currentUserService.GetUserId();

        return await _dbContext.Events
            .Where(e => e.Participants.Any(p => p.UserId == userId && p.AttendanceResponse == AttendanceResponseEnum.pending))
            .ProjectTo<EventDto>(_mapper.ConfigurationProvider)
            .ToListAsync(ct);
    }
}