using Microsoft.AspNetCore.Mvc;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TemplateService.API.Controllers;

[ApiController]
[Route("api/telegram")]
public class HelloWorldController : ControllerBase
{
    private readonly ITelegramBotClient _botClient;
    private readonly IConfiguration _config = null;

    public HelloWorldController(IConfiguration config)
    {
        _config = config;
        _botClient = new TelegramBotClient(_config["Telegram:BotToken"]);
    }

    [HttpGet("hello")]
    public async Task<IActionResult> SendHello()
    {
        try
        {
            var botToken = _config["Telegram:BotToken"];
            var chatId = _config.GetValue<long>("Telegram:ChatId");

            // 1. Проверка конфигурации
            if (string.IsNullOrEmpty(botToken))
                throw new Exception("Bot token not configured");

            Console.WriteLine($"Configuration check: Token={botToken}, ChatId={chatId}");

            // 2. Проверка доступности бота
            var botClient = new TelegramBotClient(botToken);
            var me = await botClient.GetMeAsync();
            Console.WriteLine($"Bot connection OK: {me.Username} (ID: {me.Id})");

            // 3. Отправка сообщения
            Console.WriteLine($"Sending to chat: {chatId}");
            await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: "Hello World from Swagger!");

            return Ok(new
            {
                status = "Message sent",
                bot = me.Username,
                chat_id = chatId
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ERROR: {ex}");
            return BadRequest(new
            {
                error = ex.Message,
                config = new
                {
                    token = _config["Telegram:BotToken"]?.Length,
                    chat_id = _config["Telegram:ChatId"]
                }
            });
        }
    }
}