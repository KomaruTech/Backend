﻿using TemplateService.Application.Auth.Services;
using TemplateService.Application.Event.Services;
using TemplateService.Domain.Enums;
using TemplateService.Infrastructure.Persistence;

namespace TemplateService.Application.Event.Queries;

public class ConfirmEventQueryHandler : IRequestHandler<ConfirmEventQuery, Unit>
{
    private readonly TemplateDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;
    private readonly IEventValidationService _eventValidationService;
    
    public ConfirmEventQueryHandler(
        TemplateDbContext dbContext,
        ICurrentUserService currentUserService,
        IEventValidationService eventValidationService
    )
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
        _eventValidationService = eventValidationService;
    }



    public async Task<Unit> Handle(ConfirmEventQuery request, CancellationToken cancellationToken)
    {
        var entity = await _dbContext.Events.FindAsync([request.Id], cancellationToken);
        if (entity == null)
            throw new InvalidOperationException($"Event with ID {request.Id} not found.");
        
        var userRole = _currentUserService.GetUserRole();
        
        _eventValidationService.ValidateConfirmPermissions(userRole);
        
        entity.Status = EventStatusEnum.confirmed;
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}