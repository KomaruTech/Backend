using System.ComponentModel;

namespace TemplateService.Domain.Enums;
public enum UserRoleEnum
{
    /// <summary>
    /// Обычный участник (не может создавать мероприятия)
    /// </summary>
    [Description("Обычный участник (не может создавать мероприятия) ('member')")]
    member,

    /// <summary>
    /// Организатор, но не имеет прав администратора
    /// </summary>
    [Description("Организатор, но не имеет прав администратора ('manager')")]
    manager,

    /// <summary>
    /// Администратор, обладает правами создания пользователей и чем-то еще
    /// </summary>
    [Description("Администратор, обладает правами создания пользователей и чем-то еще ('administator')")]
    administrator
}