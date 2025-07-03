using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TemplateService.Infrastructure.Persistence;

namespace TemplateService.Application.Telegram.Commands;

internal class UserConnectCommandHandler : IRequestHandler<UserConnectCommand, Unit>
{
    private readonly TemplateDbContext _dbContext;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IConfiguration _configuration;

    public UserConnectCommandHandler(
        TemplateDbContext dbContext,
        IHttpContextAccessor httpContextAccessor,
        IConfiguration configuration)
    {
        _dbContext = dbContext;
        _httpContextAccessor = httpContextAccessor;
        _configuration = configuration;
    }

    public async Task<Unit> Handle(UserConnectCommand request, CancellationToken cancellationToken)
    {
        // Проверка X-TG-API-Key в заголовках
        var httpContext = _httpContextAccessor.HttpContext;

        if (httpContext == null || !httpContext.Request.Headers.TryGetValue("X-TG-Api-Key", out var extractedApiKey))
            throw new UnauthorizedAccessException("X-TG-API-Key header is missing.");

        var configuredApiKey = _configuration["X-TG-Api-Key"];

        if (string.IsNullOrEmpty(configuredApiKey) || extractedApiKey != configuredApiKey)
            throw new UnauthorizedAccessException("Invalid X-TG-API-Key.");

        // Поиск пользователя по TelegramUserName
        var user = await _dbContext.Users
            // СДЕЛАТЬ ЧТОБЫ ДОБАВЛЯЛО ЕСЛИ СОБАКИ НЕТ
            .FirstOrDefaultAsync(u => u.TelegramUsername == "@" + request.TelegramUserName, cancellationToken);

        if (user == null)
            throw new ArgumentException($"User with TelegramUserName '{request.TelegramUserName}' not found.");

        // Обновление TelegramUserId
        user.TelegramId = request.TelegramUserId;

        await _dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}