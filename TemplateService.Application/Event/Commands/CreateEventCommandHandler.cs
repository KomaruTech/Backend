using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TemplateService.Application.Auth.Services;
using TemplateService.Application.Event.DTOs;
using TemplateService.Application.Event.Services;
using TemplateService.Domain.Entities;
using TemplateService.Domain.Enums;
using TemplateService.Infrastructure.Persistence;

namespace TemplateService.Application.Event.Commands;

internal class CreateEventCommandHandler : IRequestHandler<CreateEventCommand, EventDto>
{
    private readonly TemplateDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;
    private readonly IEventValidationService _eventValidationService;

    public CreateEventCommandHandler(
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

    public async Task<EventDto> Handle(CreateEventCommand command, CancellationToken ct)
    {
        var userId = _currentUserService.GetUserId();
        var userRole = _currentUserService.GetUserRole();

        _eventValidationService.ValidateName(command.Name);
        _eventValidationService.ValidateDescription(command.Description);
        _eventValidationService.ValidateTimeStart(command.TimeStart);
        _eventValidationService.ValidateDuration(command.TimeStart, command.TimeEnd);
        _eventValidationService.ValidateUserRole(userRole);
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
                .Distinct() // убрает дубли
                .ToList() ?? new List<string>(), // если null — сделать пустой список
            Status = EventStatusEnum.confirmed // Оно подтверждено т.к не может быть выполнено мембером
        };

        _dbContext.Events.Add(newEvent);

        // Список участников из команды (без дубликатов и без null)
        var commandParticipantIds = command.Participants?.Where(id => id != userId).Distinct().ToList() ?? new();

// Получаем список существующих пользователей по ID из команды
        var existingUserIds = await _dbContext.Users
            .Where(u => commandParticipantIds.Contains(u.Id))
            .Select(u => u.Id)
            .ToListAsync(ct);

        // Добавляем участников из команды
        foreach (var participantId in existingUserIds)
        {
            _dbContext.EventParticipants.Add(new EventParticipantEntity
            {
                EventId = id,
                UserId = participantId,
                IsSpeaker = false,
                AttendanceResponse = AttendanceResponseEnum.pending
            });
        }

        // Добавляем самого создателя в участники, если его ещё нет
        _dbContext.EventParticipants.Add(new EventParticipantEntity
        {
            EventId = id,
            UserId = userId,
            IsSpeaker = false, // при необходимости можешь сделать true
            AttendanceResponse = AttendanceResponseEnum.approved // логично, что он подтвердил
        });

        await _dbContext.SaveChangesAsync(ct);

        // Заново загружаем Event с участниками
        var fullEvent = await _dbContext.Events
            .Include(e => e.Participants)
            .FirstAsync(e => e.Id == id, ct);

        // Преобразуем в DTO уже с участниками
        return _mapper.Map<EventDto>(fullEvent);
    }
}