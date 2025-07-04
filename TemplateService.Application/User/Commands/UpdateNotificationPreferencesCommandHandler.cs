using Microsoft.EntityFrameworkCore;
using TemplateService.Application.Auth.Services;
using TemplateService.Application.PasswordService;
using TemplateService.Application.User.Dtos;
using TemplateService.Application.User.Services;
using TemplateService.Infrastructure.Persistence;

namespace TemplateService.Application.User.Commands;

internal class UpdateNotificationPreferencesCommandHandler : IRequestHandler<UpdateNotificationPreferencesCommand, UserNotificationPreferencesDto>
{
    private readonly TemplateDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;

    public UpdateNotificationPreferencesCommandHandler(
        TemplateDbContext dbContext,
        ICurrentUserService currentUserService
    )
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
    }

    public async Task<UserNotificationPreferencesDto> Handle(UpdateNotificationPreferencesCommand command, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.GetUserId();

        var user = await _dbContext.Users
                       .Include(u => u.NotificationPreferences)
                       .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken)
                   ?? throw new InvalidOperationException($"User with id {userId} not found.");

        var prefs = user.NotificationPreferences
                    ?? throw new InvalidOperationException($"User with id {userId} has no notification preferences configured.");
        
        if (command.NotifyTelegram.HasValue)
            prefs.NotifyTelegram = command.NotifyTelegram.Value;

        if (command.NotifyBeforeOneDay.HasValue)
            prefs.ReminderBefore1Day = command.NotifyBeforeOneDay.Value;

        if (command.NotifyBeforeOneHour.HasValue)
            prefs.ReminderBefore1Hour = command.NotifyBeforeOneHour.Value;

        await _dbContext.SaveChangesAsync(cancellationToken);

        return new UserNotificationPreferencesDto
        {
            Id = prefs.Id,
            NotifyTelegram = prefs.NotifyTelegram,
            NotifyBeforeOneDay = prefs.ReminderBefore1Day,
            NotifyBeforeOneHour = prefs.ReminderBefore1Hour
        };
    }
}