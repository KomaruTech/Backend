using AutoMapper;
using TemplateService.Application.User.DTOs;
using TemplateService.Domain.Entities;
using TemplateService.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TemplateService.Application.PasswordService;

namespace TemplateService.Application.User.Commands;

internal class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, UserDto>
{
    private readonly TemplateDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly IPasswordHasher _passwordHasher;

    public CreateUserCommandHandler(
        TemplateDbContext dbContext,
        IMapper mapper,
        IPasswordHasher passwordHasher)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _passwordHasher = passwordHasher;
    }

    public async Task<UserDto> Handle(CreateUserCommand command, CancellationToken ct)
    {
        // Генерируем общий ID для связки
        var id = Guid.NewGuid();

        var notificationPreferences = new NotificationPreferencesEntity
        {
            Id = id
            // Остальные поля не заполняем, они возьмутся из БД по DEFAULT
        };

        var user = new UserEntity
        {
            Id = id,
            Login = command.Login,
            Name = command.Name,
            Surname = command.Surname,
            Email = command.Email,
            NotificationPreferencesId = id,
            NotificationPreferences = notificationPreferences,
            PasswordHash = _passwordHasher.HashPassword(command.Password)
        };

        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync(ct);

        return _mapper.Map<UserDto>(user);
    }
}