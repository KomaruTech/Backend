using Microsoft.EntityFrameworkCore;
using TemplateService.Application.Event.Services;

namespace TemplateService.Application.Event.Commands;

using AutoMapper;
using DTOs;
using Domain.Entities;
using Infrastructure.Persistence;
using MediatR;
using PasswordService;
using Microsoft.AspNetCore.Http;
using Auth.Services;
using Domain.Enums;

internal class CreateEventCommandHandler : IRequestHandler<CreateEventCommand, EventDto>
{
    private readonly TemplateDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;
    private readonly IEventFieldValidationService _eventFieldValidationService;

    public CreateEventCommandHandler(
        TemplateDbContext dbContext,
        IMapper mapper,
        IHttpContextAccessor httpContextAccessor,
        ICurrentUserService currentUserService,
        IEventFieldValidationService eventFieldValidationService
    )
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _currentUserService = currentUserService;
        _eventFieldValidationService = eventFieldValidationService;
    }

    public async Task<EventDto> Handle(CreateEventCommand command, CancellationToken ct)
    {
        var userId = _currentUserService.GetUserId();
        var userRole = _currentUserService.GetUserRole();

        _eventFieldValidationService.ValidateName(command.Name);
        _eventFieldValidationService.ValidateDescription(command.Description);
        _eventFieldValidationService.ValidateTimeStart(command.TimeStart);
        _eventFieldValidationService.ValidateDuration(command.TimeStart, command.TimeEnd);
        _eventFieldValidationService.ValidateUserRole(userRole);
        _eventFieldValidationService.ValidateLocation(command.Location);
        
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
                    AttendanceMarked = false
                };
                _dbContext.EventParticipants.Add(participantEntity);
            }
        }

        await _dbContext.SaveChangesAsync(ct);
        return _mapper.Map<EventDto>(newEvent);
    }
}