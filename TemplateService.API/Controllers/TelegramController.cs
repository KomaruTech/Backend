
using System.Net.Mime;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types.Enums;
using TemplateService.Application.Telegram.Commands;
using TemplateService.Application.TelegramService;



namespace TemplateService.API.Controllers;
/// <summary>
/// Связывание ТГ и пользователя
/// </summary>
[ApiController]
[AllowAnonymous]
[Produces(MediaTypeNames.Application.Json)]
[Route("api/v1/[controller]")]
public class TelegramController : ControllerBase
{
    private readonly IMediator _mediator;

    public TelegramController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Получение Username пользователя, и его ID (только от ТГ БОТА)
    /// </summary>
    [HttpPost("connect")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ConnectTgUserNameWithId([FromBody] UserConnectCommand command)
    {
        await _mediator.Send(command);
        return Ok();
    }

    [HttpPost("notifications/send")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SendNotification(
     [FromBody] SendToTelegramEventDto notificationDto,
     [FromServices] ITelegramBotClient botClient,
     [FromServices] ILogger<TelegramController> logger) // Добавляем логгер
    {
        try
        {
            // Валидация входящих данных
            if (string.IsNullOrWhiteSpace(notificationDto.Name))
            {
                logger.LogWarning("Пустое название события для EventId: {EventId}", notificationDto.EventId);
                return BadRequest("Поле Name обязательно для заполнения");
            }

            if (string.IsNullOrWhiteSpace(notificationDto.Description))
            {
                logger.LogWarning("Пустое описание события для EventId: {EventId}", notificationDto.EventId);
                return BadRequest("Поле Description обязательно для заполнения");
            }

            // Форматирование сообщения
            var message = FormatNotificationMessage(notificationDto);

            // Отправка сообщения
            await botClient.SendTextMessageAsync(
                chatId: notificationDto.TelegramUserId,
                text: message,
                parseMode: ParseMode.Html,
                cancellationToken: HttpContext.RequestAborted);

            logger.LogInformation("Уведомление отправлено для EventId: {EventId}, UserId: {UserId}",
                notificationDto.EventId, notificationDto.TelegramUserId);

            return Ok(new { success = true });
        }
        catch (ApiRequestException telegramEx)
        {
            logger.LogError(telegramEx, "Ошибка Telegram API при отправке уведомления. EventId: {EventId}, ErrorCode: {ErrorCode}",
                notificationDto.EventId, telegramEx.ErrorCode);
            return StatusCode(StatusCodes.Status502BadGateway, "Ошибка при отправке через Telegram API");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ошибка при отправке уведомления. EventId: {EventId}", notificationDto.EventId);
            return StatusCode(StatusCodes.Status500InternalServerError, "Внутренняя ошибка сервера");
        }
    }

    private string FormatNotificationMessage(SendToTelegramEventDto dto)
    {
        var timeLeft = dto.TimeStart - DateTime.UtcNow;
        string timeLeftText;

        if (timeLeft.TotalDays >= 1)
            timeLeftText = $"Через {(int)timeLeft.TotalDays} дней ({dto.TimeStart:dd.MM.yyyy HH:mm})";
        else if (timeLeft.TotalHours >= 1)
            timeLeftText = $"Через {(int)timeLeft.TotalHours} часов ({dto.TimeStart:HH:mm})";
        else
            timeLeftText = $"Скоро ({dto.TimeStart:HH:mm})";

        return $"🔔 <b>{dto.Name}</b>\n\n" +
               $"⏰ {timeLeftText}\n" +
               $"📍 {dto.Location ?? "Место не указано"}\n\n" +
               $"{dto.Description}\n\n" +
               $"<a href=\"https://your-service.com/events/{dto.EventId}\">Подробнее</a>";
    }
}
