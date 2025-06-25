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
    Pending,

    /// <summary>
    /// Персональные встречи
    /// </summary>
    [Description("Заявка одобрена ('approved')")]
    Approved,

    /// <summary>
    /// Групповые мероприятия
    /// </summary>
    [Description("Заявка отклонена ('rejected')")]
    Rejected
}