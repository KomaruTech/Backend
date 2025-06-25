using System.ComponentModel;

namespace TemplateService.Domain.Enums;

/// <summary>
/// Тип мета-данных
/// </summary>
public enum EventTypeEnum
{
    /// <summary>
    /// Общие мероприятия
    /// </summary>
    [Description("Общее мероприятие ('general')")]
    General,

    /// <summary>
    /// Персональные встречи
    /// </summary>
    [Description("Персональная встреча ('personal')")]
    Personal,

    /// <summary>
    /// Групповые мероприятия
    /// </summary>
    [Description("Групповое мероприятие ('group')")]
    Group
}