using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TemplateService.Application.Auth.Services;
using TemplateService.Application.User.Dtos;
using TemplateService.Application.User.DTOs;
using TemplateService.Application.User.Services;
using TemplateService.Infrastructure.Persistence;

namespace TemplateService.Application.User.Queries;

internal class GetNotificationPreferencesQueryHandler : IRequestHandler<GetNotificationPreferencesQuery, UserNotificationPreferencesDto>
{
    private readonly TemplateDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;

    public GetNotificationPreferencesQueryHandler(
        TemplateDbContext dbContext,
        IMapper mapper,
        ICurrentUserService currentUserService
    )
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _currentUserService = currentUserService;
    }

    public async Task<UserNotificationPreferencesDto> Handle(GetNotificationPreferencesQuery query, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.GetUserId();
        
        var userNotificationPreferences = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken)
                   ?? throw new InvalidOperationException($"User with id {userId} not found.");

        return _mapper.Map<UserNotificationPreferencesDto>(userNotificationPreferences);
    }
}