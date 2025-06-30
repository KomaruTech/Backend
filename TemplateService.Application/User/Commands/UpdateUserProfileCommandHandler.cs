using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TemplateService.Application.Auth.Services;
using TemplateService.Application.User.DTOs;
using TemplateService.Application.User.Services;
using TemplateService.Infrastructure.Persistence;

namespace TemplateService.Application.User.Commands;

internal class UpdateUserProfileCommandHandler : IRequestHandler<UpdateUserProfileCommand, UserDto>
{
    private readonly TemplateDbContext _dbContext;
    private readonly IUserValidationService _userValidationService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;
    private readonly IUserHelperService _userHelperService;

    public UpdateUserProfileCommandHandler(
        TemplateDbContext dbContext,
        IUserValidationService userValidationService,
        ICurrentUserService currentUserService,
        IMapper mapper,
        IUserHelperService userHelperService
    )
    {
        _dbContext = dbContext;
        _userValidationService = userValidationService;
        _currentUserService = currentUserService;
        _mapper = mapper;
        _userHelperService = userHelperService;
    }

    public async Task<UserDto> Handle(UpdateUserProfileCommand command, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.GetUserId();

        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken)
                   ?? throw new InvalidOperationException($"User with id {userId} not found.");


        if (command.Name != null)
            _userValidationService.ValidateName(command.Name);
        if (command.Surname != null)
            _userValidationService.ValidateSurname(command.Surname);
        if (command.Email != null)
            _userValidationService.ValidateEmail(command.Email);

        var telegramUsername = command.TelegramUsername;

        if (telegramUsername != null)
        {
            telegramUsername = telegramUsername.StartsWith('@') ? telegramUsername : '@' + telegramUsername;
            _userValidationService.ValidateTelegramUsername(telegramUsername);

            // Проверяем уникальность telegramUsername среди других пользователей
            var exists = await _dbContext.Users
                .AnyAsync(u => u.TelegramUsername == telegramUsername && u.Id != userId, cancellationToken);
            if (exists)
                throw new ArgumentException($"Telegram username '{telegramUsername}' is already taken by another user.");
        }

        command = command with { TelegramUsername = telegramUsername };

        _mapper.Map(command, user);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return _userHelperService.BuildUserDto(user, _mapper);
    }
}