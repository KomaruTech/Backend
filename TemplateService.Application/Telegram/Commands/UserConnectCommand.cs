namespace TemplateService.Application.Telegram.Commands;

public record UserConnectCommand(
    long TelegramUserId,
    string TelegramUserName
) : IRequest<Unit>;