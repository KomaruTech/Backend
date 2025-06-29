namespace TemplateService.Application.User.Commands;

using AutoMapper;
using TemplateService.Application.Auth.Services;
using DTOs;

using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Services;

internal class UpdateUserProfileCommandHandler : IRequestHandler<UpdateUserProfileCommand, UserDto>
{
    private readonly TemplateDbContext _dbContext;
    private readonly IUserValidationService _userValidationService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;

    public UpdateUserProfileCommandHandler(
        TemplateDbContext dbContext,
        IUserValidationService userValidationService,
        ICurrentUserService currentUserService,
        IMapper mapper
    )
    {
        _dbContext = dbContext;
        _userValidationService = userValidationService;
        _currentUserService = currentUserService;
        _mapper = mapper;
    }

    public async Task<UserDto> Handle(UpdateUserProfileCommand command, CancellationToken ct)
    {
        var userId = _currentUserService.GetUserId();

        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId, ct);

        if (user == null)
            throw new ArgumentException($"User with ID {userId} not found");


        if (command.Name != null)
            _userValidationService.ValidateName(command.Name);
        if (command.Surname != null)
            _userValidationService.ValidateSurname(command.Surname);
        if (command.Email != null)
            _userValidationService.ValidateEmail(command.Email);

        var telegramId = command.TelegramId;

        if (!string.IsNullOrWhiteSpace(telegramId))
        {
            telegramId = telegramId.StartsWith('@') ? telegramId : '@' + telegramId;
            _userValidationService.ValidateTelegram(telegramId);
        }
        
        command = command with { TelegramId = telegramId };
        
        _mapper.Map(command, user);
        
        await _dbContext.SaveChangesAsync(ct);

        return _mapper.Map<UserDto>(user);
    }
}