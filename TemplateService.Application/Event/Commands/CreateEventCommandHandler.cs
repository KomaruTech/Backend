using System.Security.Claims;
using TemplateService.Domain.Enums;

namespace TemplateService.Application.Event.Commands;

using AutoMapper;
using TemplateService.Application.Event.DTOs;
using TemplateService.Domain.Entities;
using TemplateService.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TemplateService.Application.PasswordService;
using Microsoft.AspNetCore.Http;
internal class CreateEventCommandHandler : IRequestHandler<CreateEventCommand, EventDto>
{
    private readonly TemplateDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IHttpContextAccessor _httpContextAccessor;
    
    public CreateEventCommandHandler(
        TemplateDbContext dbContext,
        IMapper mapper,
        IPasswordHasher passwordHasher,
        IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _passwordHasher = passwordHasher;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<EventDto> Handle(CreateEventCommand command, CancellationToken ct)
    {
        var jwtUser = _httpContextAccessor.HttpContext?.User;
        
        var idClaim = jwtUser.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        if (!Guid.TryParse(idClaim, out var userId))
            throw new UnauthorizedAccessException("Неверный формат userId в токене");
        
        var roleClaim = jwtUser.FindFirst(ClaimTypes.Role)?.Value;
        
        if (roleClaim == null)
            throw new UnauthorizedAccessException("Not found role in token");
        
        if (!Enum.TryParse<UserRoleEnum>(roleClaim, ignoreCase: true, out var userRole))
            throw new UnauthorizedAccessException("InvalidRole");

        if (userRole == UserRoleEnum.member)
            throw new InvalidOperationException("Пользователь с ролью 'member' не может создавать мероприятия");

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