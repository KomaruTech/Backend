using System.ComponentModel;

namespace TemplateService.Domain.Enums;

/// <summary>
/// Мероприятие
/// </summary>
public enum EventStatusEnum
{
    /// <summary>
    /// Общие мероприятия
    /// </summary>
    [Description("Мероприятие одобрено ('confirmed')")]
    confirmed,

    /// <summary>
    /// Мероприятие предложено (но еще не одобрено)
    /// </summary>
    [Description("Персональная встреча ('suggested')")]
    suggested
}