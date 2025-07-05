using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TemplateService.Application.Auth.Services;
using TemplateService.Application.EventFeedback.DTOs;
using TemplateService.Domain.Entities;
using TemplateService.Domain.Enums;
using TemplateService.Infrastructure.Persistence;

namespace TemplateService.Application.EventFeedback.Commands;

internal class AddEventFeedbackCommandHandler : IRequestHandler<AddEventFeedbackCommand, EventFeedbackDto>
{
    private readonly TemplateDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;

    public AddEventFeedbackCommandHandler(
        TemplateDbContext dbContext,
        ICurrentUserService currentUserService,
        IMapper mapper
    )
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
        _mapper = mapper;
    }

    public async Task<EventFeedbackDto> Handle(AddEventFeedbackCommand command, CancellationToken cancellationToken)
    {
        if (command.Rating is < 1 or > 5)
            throw new ArgumentException("Rating must be between 1 and 5.");
        
        var userId = _currentUserService.GetUserId();
        
        var currentEvent = await _dbContext.Events
                               .FirstOrDefaultAsync(e => e.Id == command.EventId, cancellationToken)
                           ?? throw new InvalidOperationException($"Event with id {command.EventId} not found.");

        var isParticipant = await _dbContext.EventParticipants.AnyAsync(
            p => p.EventId == command.EventId
                 && p.UserId == userId
                 && p.AttendanceResponse == AttendanceResponseEnum.approved,
            cancellationToken);

        if (!isParticipant)
            throw new InvalidOperationException("User is not a confirmed participant of the event.");

        var existingFeedback = await _dbContext.EventFeedbacks.FirstOrDefaultAsync(
            f => f.EventId == command.EventId && f.UserId == userId,
            cancellationToken);

        if (existingFeedback != null)
            throw new InvalidOperationException("Feedback has already been submitted for this event by this user.");

        var feedback = new EventFeedbackEntity
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            EventId = command.EventId,
            Rating = command.Rating,
            Comment = command.Comment,
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.EventFeedbacks.Add(feedback);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return _mapper.Map<EventFeedbackDto>(feedback);
    }
}