using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using TemplateService.Application.Auth.Services;
using TemplateService.Application.Event.DTOs;
using TemplateService.Application.Event.Services;
using TemplateService.Domain.Enums;
using TemplateService.Infrastructure.Persistence;

namespace TemplateService.Application.Event.Queries;

internal class GetSuggestedEventsQueryHandler : IRequestHandler<GetSuggestedEventsQuery, List<EventDto>>
{
    private readonly TemplateDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;
    private readonly IEventValidationService _eventValidationService;

    public GetSuggestedEventsQueryHandler(
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

    public async Task<List<EventDto>> Handle(GetSuggestedEventsQuery query, CancellationToken ct)
    {
        var userRole = _currentUserService.GetUserRole();

        _eventValidationService.ValidateConfirmPermissions(userRole);
        
        return await _dbContext.Events
            .Where(u => u.Status == EventStatusEnum.suggested)
            .ProjectTo<EventDto>(_mapper.ConfigurationProvider)
            .ToListAsync(ct);
    }
}