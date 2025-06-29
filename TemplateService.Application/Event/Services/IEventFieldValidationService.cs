namespace TemplateService.Application.Event.Services;

using TemplateService.Domain.Enums;
using TemplateService.Application.Event.Commands;

public interface IEventFieldValidationService
{
    void ValidateName(string? name);
    void ValidateDescription(string? description);
    void ValidateUserRole(UserRoleEnum userRole);
    void ValidateTimeStart(DateTime timeStart);
    void ValidateDuration(DateTime timeStart, DateTime? timeEnd);
    void ValidateLocation(string? location);
    void ValidateUpdatePermissions(Guid userId, Guid createdById, UserRoleEnum userRole);
}
