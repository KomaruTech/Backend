using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using TemplateService.Application.Auth.Services;
using TemplateService.Application.Event.DTOs;
using TemplateService.Application.Event.Services;
using TemplateService.Domain.Enums;
using TemplateService.Infrastructure.Persistence;

namespace TemplateService.Application.Event.Queries;

internal class GetMyEventsQueryHandler : IRequestHandler<GetMyEventsQuery, List<EventDto>>
{
    private readonly TemplateDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;
    private readonly IEventValidationService _eventValidationService;

    public GetMyEventsQueryHandler(
        TemplateDbContext dbContext,
        IMapper mapper,
        ICurrentUserService currentUserService,
        IEventValidationService eventValidationService
    )
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _currentUserService = currentUserService;
        _eventValidationService = eventValidationService;
    }

    public async Task<List<EventDto>> Handle(GetMyEventsQuery query, CancellationToken ct)
    {
        var userId = _currentUserService.GetUserId();

        return await _dbContext.Events
            .Where(e => e.Participants.Any(p => p.UserId == userId && p.AttendanceResponse == AttendanceResponseEnum.approved))
            .ProjectTo<EventDto>(_mapper.ConfigurationProvider)
            .ToListAsync(ct);
    }
}