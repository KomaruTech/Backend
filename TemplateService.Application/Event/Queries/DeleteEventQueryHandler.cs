using TemplateService.Application.Auth.Services;
using TemplateService.Application.Event.Services;
using TemplateService.Infrastructure.Persistence;

namespace TemplateService.Application.Event.Queries;

public class DeleteEventQueryHandler : IRequestHandler<DeleteEventQuery, Unit>
{
    private readonly TemplateDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;
    private readonly IEventValidationService _eventValidationService;

    public DeleteEventQueryHandler(
        TemplateDbContext dbContext,
        ICurrentUserService currentUserService,
        IEventValidationService eventValidationService
    )
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
        _eventValidationService = eventValidationService;
    }


    public async Task<Unit> Handle(DeleteEventQuery request, CancellationToken cancellationToken)
    {
        var entity = await _dbContext.Events.FindAsync([request.Id], cancellationToken);
        if (entity == null)
            throw new InvalidOperationException($"Event with ID {request.Id} not found.");

        var userId = _currentUserService.GetUserId();
        var userRole = _currentUserService.GetUserRole();

        _eventValidationService.ValidateUpdatePermissions(userId, entity.CreatedById, userRole);

        _dbContext.Events.Remove(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}