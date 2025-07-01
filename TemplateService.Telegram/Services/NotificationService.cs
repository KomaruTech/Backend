#nullable enable
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TemplateService.Domain.Entities;
using TemplateService.Infrastructure.Persistence;

namespace TemplateService.Telegram.Services;

public class NotificationService
{
    private readonly TemplateDbContext _dbContext;
    private readonly ILogger<NotificationService> _logger;
    private readonly TelegramService _telegramService;

    public NotificationService(
        TemplateDbContext dbContext,
        ILogger<NotificationService> logger,
        TelegramService telegramService)
    {
        _dbContext = dbContext;
        _logger = logger;
        _telegramService = telegramService;
    }

    public async Task CheckEventsAndSendNotifications()
    {
        var now = DateTime.UtcNow;

        var events = await _dbContext.Events
            .Include(e => e.CreatedBy)
            .Where(e => e.TimeStart > now && e.TimeStart < now.AddHours(25))
            .ToListAsync();

        foreach (var evt in events)
        {
            // Проверяем уведомление за 1 день
            if (evt.TimeStart.AddDays(-1) <= now && now < evt.TimeStart)
            {
                await SendNotification(
                    evt.CreatedBy,
                    $"Напоминание: {evt.Name} начнётся через 1 день ({evt.TimeStart:dd.MM.yyyy HH:mm})");
            }

            // Проверяем уведомление за 1 час
            if (evt.TimeStart.AddHours(-1) <= now && now < evt.TimeStart)
            {
                await SendNotification(
                    evt.CreatedBy,
                    $"Скоро начало: {evt.Name} через 1 час!");
            }
        }
    }

    // Добавляем async и возвращаем Task
    private async Task SendNotification(UserEntity user, string message)
    {
        if (user.TelegramId == null)
        {
            _logger.LogWarning("У пользователя {UserId} не указан Telegram ID", user.Id);
            return;
        }

        await _telegramService.SendMessage(user.TelegramId.Value, message);
    }
}