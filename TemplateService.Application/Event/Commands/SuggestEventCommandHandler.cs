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

public class SuggestEventCommandHandler : IRequestHandler<SuggestEventCommand, EventDto>
{
    private readonly TemplateDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;
    private readonly IEventValidationService _eventValidationService;

    public SuggestEventCommandHandler(
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


    public async Task<EventDto> Handle(SuggestEventCommand command, CancellationToken ct)
    {
        var userId = _currentUserService.GetUserId();
        
        _eventValidationService.ValidateName(command.Name);
        _eventValidationService.ValidateDescription(command.Description);
        _eventValidationService.ValidateTimeStart(command.TimeStart);
        _eventValidationService.ValidateDuration(command.TimeStart, command.TimeEnd);
        _eventValidationService.ValidateLocation(command.Location);

        
        // Генерируем общий ID для связки
        var id = Guid.NewGuid();

        var newEvent = new EventEntity
        {
            Id = id,
            Name = command.Name,
            Description = command.Description,
            TimeStart = command.TimeStart,
            TimeEnd = command.TimeEnd,
            CreatedById = userId,
            Type = command.Type,
            Location = command.Location,
            Keywords = command.Keywords?
                .Where(k => !string.IsNullOrWhiteSpace(k)) // фильтрация пустых
                .Select(k => k.ToLowerInvariant().Trim()) // приведение к нижнему регистру и обрезка пробелов
                .Distinct() // убирает дубли
                .ToList() ?? new List<string>(), // если null — сделать пустой список
            Status = EventStatusEnum.suggested // Оно предложено, и может быть одобрено
        };

        _dbContext.Events.Add(newEvent);

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
                    EventId = id,
                    UserId = participantId,
                    IsSpeaker = false,
                    AttendanceResponse = AttendanceResponseEnum.pending
                };
                _dbContext.EventParticipants.Add(participantEntity);
            }
        }
        
        await _dbContext.SaveChangesAsync(ct);
        return _mapper.Map<EventDto>(newEvent);
    }
}