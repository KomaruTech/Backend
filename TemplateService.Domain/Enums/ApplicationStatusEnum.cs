using System.ComponentModel;

namespace TemplateService.Domain.Enums;

/// <summary>
/// Тип мета-данных
/// </summary>
public enum ApplicationStatusEnum
{
    /// <summary>
    /// Заявка на рассмотрении
    /// </summary>
    [Description("Заявка на рассмотрении ('pending')")]
    pending,

    /// <summary>
    /// Персональные встречи
    /// </summary>
    [Description("Заявка одобрена ('approved')")]
    approved,

    /// <summary>
    /// Групповые мероприятия
    /// </summary>
    [Description("Заявка отклонена ('rejected')")]
    rejected
}