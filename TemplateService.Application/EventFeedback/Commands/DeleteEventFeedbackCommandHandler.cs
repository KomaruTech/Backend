using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TemplateService.Application.Auth.Services;
using TemplateService.Application.EventFeedback.DTOs;
using TemplateService.Infrastructure.Persistence;

namespace TemplateService.Application.EventFeedback.Commands;

internal class DeleteEventFeedbackCommandHandler : IRequestHandler<DeleteEventFeedbackCommand, Unit>
{
    private readonly TemplateDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;

    public DeleteEventFeedbackCommandHandler(
        TemplateDbContext dbContext,
        ICurrentUserService currentUserService,
        IMapper mapper
    )
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
        _mapper = mapper;
    }

    public async Task<Unit> Handle(DeleteEventFeedbackCommand command, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.GetUserId();

        var currentEvent = await _dbContext.Events
                               .FirstOrDefaultAsync(e => e.Id == command.EventId, cancellationToken)
                           ?? throw new InvalidOperationException($"Event with id {command.EventId} not found.");

        var existingFeedback = await _dbContext.EventFeedbacks.FirstOrDefaultAsync(
            f => f.EventId == command.EventId && f.UserId == userId,
            cancellationToken);

        if (existingFeedback == null)
            throw new InvalidOperationException("User hasn't got feedback for this event.");

        _dbContext.EventFeedbacks.Remove(existingFeedback);

        await _dbContext.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}