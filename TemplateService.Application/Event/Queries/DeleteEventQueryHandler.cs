using AutoMapper;
using TemplateService.Application.Auth.Services;
using TemplateService.Domain.Enums;
using TemplateService.Infrastructure.Persistence;

namespace TemplateService.Application.Event.Queries;

public class DeleteEventQueryHandler : IRequestHandler<DeleteEventQuery, Unit>
{
    private readonly TemplateDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;
    
    public DeleteEventQueryHandler(
        TemplateDbContext dbContext,
        IMapper mapper,
        ICurrentUserService currentUserService
    )
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _currentUserService = currentUserService;
    }


    public async Task<Unit> Handle(DeleteEventQuery request, CancellationToken cancellationToken)
    {
        var entity = await _dbContext.Events.FindAsync([request.Id], cancellationToken);
        if (entity == null)
            throw new InvalidOperationException($"Event with ID {request.Id} not found.");

        var userId = _currentUserService.GetUserId();
        var userRole = _currentUserService.GetUserRole();

        // Проверка прав доступа: либо админ, либо создатель мероприятия
        if (entity.CreatedById != userId && userRole != UserRoleEnum.administrator)
            throw new UnauthorizedAccessException("You do not have permission to delete this event.");

        _dbContext.Events.Remove(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}