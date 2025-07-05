using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TemplateService.Application.Auth.Services;
using TemplateService.Application.EventFeedback.DTOs;
using TemplateService.Infrastructure.Persistence;

namespace TemplateService.Application.EventFeedback.Commands;

internal class EditEventFeedbackCommandHandler : IRequestHandler<EditEventFeedbackCommand, EventFeedbackDto>
{
    private readonly TemplateDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;

    public EditEventFeedbackCommandHandler(
        TemplateDbContext dbContext,
        ICurrentUserService currentUserService,
        IMapper mapper
    )
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
        _mapper = mapper;
    }

    public async Task<EventFeedbackDto> Handle(EditEventFeedbackCommand command, CancellationToken cancellationToken)
    {
        
        if (command.Rating is < 1 or > 5)
            throw new ArgumentException("Rating must be between 1 and 5.");
        
        var userId = _currentUserService.GetUserId();
        
        var currentEvent = await _dbContext.Events
                               .FirstOrDefaultAsync(e => e.Id == command.EventId, cancellationToken)
                           ?? throw new InvalidOperationException($"Event with id {command.EventId} not found.");

        var existingFeedback = await _dbContext.EventFeedbacks.FirstOrDefaultAsync(
            f => f.EventId == command.EventId && f.UserId == userId,
            cancellationToken);

        if (existingFeedback == null)
            throw new InvalidOperationException("User hasn't got feedback for this event.");
        
        existingFeedback.Rating = command.Rating;
        existingFeedback.Comment = command.Comment;

        await _dbContext.SaveChangesAsync(cancellationToken);
        
        return _mapper.Map<EventFeedbackDto>(existingFeedback);
    }
}