#nullable enable
using Microsoft.AspNetCore.Mvc;
using TemplateService.Telegram.Services;

namespace TemplateService.API.Controllers;

[ApiController]
[Route("api/telegram")]
public class TelegramController : ControllerBase
{
    private readonly TelegramService _telegramService;
    private readonly ILogger<TelegramController> _logger;

    public TelegramController(
        TelegramService telegramService,
        ILogger<TelegramController> logger)
    {
        _telegramService = telegramService;
        _logger = logger;
    }

    [HttpPost("test")]
    public async Task<IActionResult> SendTestMessage([FromQuery] long chatId)
    {
        try
        {
            // Добавляем await
            await _telegramService.SendMessage(chatId, "✅ Это тестовое сообщение от бота");
            return Ok(new { status = "success", message = "Сообщение отправлено" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка отправки тестового сообщения");
            return StatusCode(500, new { status = "error", message = ex.Message });
        }
    }
}