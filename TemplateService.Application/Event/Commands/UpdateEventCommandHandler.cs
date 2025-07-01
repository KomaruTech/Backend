using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using TemplateService.Application.Auth.Services;
using TemplateService.Application.Event.DTOs;
using TemplateService.Application.Event.Services;
using TemplateService.Domain.Entities;
using TemplateService.Domain.Enums;
using TemplateService.Infrastructure.Persistence;

namespace TemplateService.Application.Event.Commands;

internal class UpdateEventCommandHandler : IRequestHandler<UpdateEventCommand, EventDto>
{
    private readonly TemplateDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;
    private readonly IEventValidationService _eventValidationService;

    public UpdateEventCommandHandler(
        TemplateDbContext dbContext,
        IMapper mapper,
        IHttpContextAccessor httpContextAccessor,
        ICurrentUserService currentUserService,
        IEventValidationService eventValidationService
    )
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _currentUserService = currentUserService;
        _eventValidationService = eventValidationService;
    }

    public async Task<EventDto> Handle(UpdateEventCommand command, CancellationToken ct)
    {
        var existingEvent = await _dbContext.Events
            .FirstOrDefaultAsync(e => e.Id == command.Id, ct);

        if (existingEvent == null)
            throw new ArgumentException($"Event with ID {command.Id} not found.");


        var userId = _currentUserService.GetUserId();
        var userRole = _currentUserService.GetUserRole();

        if (command.Name != null)
            _eventValidationService.ValidateName(command.Name);
        if (command.Description != null)
            _eventValidationService.ValidateDescription(command.Description);
        if (command.TimeStart.HasValue)
            _eventValidationService.ValidateTimeStart(command.TimeStart.Value);
        if (command.TimeStart.HasValue)
            _eventValidationService.ValidateDuration(command.TimeStart.Value, command.TimeEnd);

        if (command.Location != null)
            _eventValidationService.ValidateLocation(command.Location);

        _eventValidationService.ValidateUpdatePermissions(userId, existingEvent.CreatedById, userRole);

        var updatedCommand = command;

        if (command.Keywords is { Count: > 0 })
        {
            var cleanedKeywords = command.Keywords
                .Where(k => !string.IsNullOrWhiteSpace(k))
                .Select(k => k.ToLowerInvariant().Trim())
                .Distinct()
                .ToList();

            // Присваиваем только если что-то валидное осталось
            if (cleanedKeywords.Count > 0)
                updatedCommand = command with { Keywords = cleanedKeywords };
        }

        _mapper.Map(updatedCommand, existingEvent);

        // Сохраняем
        await _dbContext.SaveChangesAsync(ct);


        // Добавляем участников, если они есть
        if (command.Participants != null && command.Participants.Any())
        {
            // Получаем список существующих пользователей по ID из команды (одним запросом)
            var existingUserIds = await _dbContext.Users
                .Where(u => command.Participants.Contains(u.Id))
                .Select(u => u.Id)
                .ToListAsync(ct);

            // Добавляем только существующих пользователей
            foreach (var participantId in existingUserIds)
            {
                var participantEntity = new EventParticipantEntity
                {
                    EventId = existingEvent.Id,
                    UserId = participantId,
                    IsSpeaker = false,
                    AttendanceResponse = AttendanceResponseEnum.pending
                };
                _dbContext.EventParticipants.Add(participantEntity);
            }
        }

        await _dbContext.SaveChangesAsync(ct);
        return _mapper.Map<EventDto>(existingEvent);
    }
}