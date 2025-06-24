using System.ComponentModel;

namespace TemplateService.Domain.Enums;

/// <summary>
/// Тип мета-данных
/// </summary>
public enum MetaTypeEnum
{
    /// <summary>
    /// ID документа в СЭД АТАЧ
    /// </summary>
    [Description("ID документа в СЭД АТАЧ")]
    AtachDocumentId = 1,

    /// <summary>
    /// Регистрационный номер
    /// </summary>
    [Description("Регистрационный номер")]
    RegNumber = 2,

    /// <summary>
    /// Дата регистрации
    /// </summary>
    [Description("Дата регистрации")]
    RegDate = 3    
}