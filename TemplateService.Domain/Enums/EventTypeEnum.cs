using System.ComponentModel;

namespace TemplateService.Domain.Enums;

/// <summary>
/// Тип мероприятия
/// </summary>
public enum EventTypeEnum
{
    /// <summary>
    /// Общие мероприятия
    /// </summary>
    [Description("Общее мероприятие ('general')")]
    general,

    /// <summary>
    /// Персональные встречи
    /// </summary>
    [Description("Персональная встреча ('personal')")]
    personal,

    /// <summary>
    /// Групповые мероприятия
    /// </summary>
    [Description("Групповое мероприятие ('group')")]
    group
}