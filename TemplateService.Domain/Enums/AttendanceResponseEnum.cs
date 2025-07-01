using System.ComponentModel;

namespace TemplateService.Domain.Enums;

public enum AttendanceResponseEnum
{
    /// <summary>
    /// Ждем ответа
    /// </summary>
    [Description("Приглашение на рассмотрении ('pending')")]
    pending,

    /// <summary>
    /// Приглашение принято
    /// </summary>
    [Description("Приглашение принято ('approved')")]
    approved,

    /// <summary>
    /// Приглашение не принято
    /// </summary>
    [Description("Приглашение не принято ('rejected')")]
    rejected
}