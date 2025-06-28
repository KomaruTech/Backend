using System.Security.Claims;
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
    private readonly IHttpContextAccessor _httpContextAccessor;
    
    public CreateEventCommandHandler(
        TemplateDbContext dbContext,
        IMapper mapper,
        IPasswordHasher passwordHasher,
        IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<EventDto> Handle(CreateEventCommand command, CancellationToken ct)
    {
        var jwtUser = _httpContextAccessor.HttpContext?.User;

        var idClaim = jwtUser.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        if (!Guid.TryParse(idClaim, out var userId))
            throw new UnauthorizedAccessException("Incorrect UserId");

        var roleClaim = jwtUser.FindFirst(ClaimTypes.Role)?.Value;
        if (!Enum.TryParse<UserRoleEnum>(roleClaim, ignoreCase: true, out var userRole))
            throw new UnauthorizedAccessException("Incorrect Role");

        if (userRole == UserRoleEnum.member)
            throw new InvalidOperationException("User with 'member' role can't create events");
        
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