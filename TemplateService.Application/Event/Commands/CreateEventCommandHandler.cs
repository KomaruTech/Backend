using TemplateService.Application.Auth.Services;
using TemplateService.Domain.Enums;

namespace TemplateService.Application.Event.Commands;

using AutoMapper;
using DTOs;
using Domain.Entities;
using Infrastructure.Persistence;
using MediatR;
using PasswordService;
using Microsoft.AspNetCore.Http;

internal class CreateEventCommandHandler : IRequestHandler<CreateEventCommand, EventDto>
{
    private readonly TemplateDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;

    public CreateEventCommandHandler(
        TemplateDbContext dbContext,
        IMapper mapper,
        IPasswordHasher passwordHasher,
        IHttpContextAccessor httpContextAccessor,
        ICurrentUserService currentUserService
    )
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _currentUserService = currentUserService;
    }

    public async Task<EventDto> Handle(CreateEventCommand command, CancellationToken ct)
    {
        var userId = _currentUserService.GetUserId();
        var userRole = _currentUserService.GetUserRole();

        if (userRole == UserRoleEnum.member)
            throw new InvalidOperationException("User with role 'member' cant create events.");

        //Валидация длительности, минимум 10 минут на мероприятие если указан конец.
        if (command.TimeEnd.HasValue)
        {
            var duration = command.TimeEnd.Value - command.TimeStart;
            if (duration < TimeSpan.FromMinutes(10))
                throw new ArgumentException("The end time should be at least 10 minutes after the event starts.");
        }

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
            Keywords = command.Keywords,
        };

        _dbContext.Events.Add(newEvent);
        await _dbContext.SaveChangesAsync(ct);

        return _mapper.Map<EventDto>(newEvent);
    }
}