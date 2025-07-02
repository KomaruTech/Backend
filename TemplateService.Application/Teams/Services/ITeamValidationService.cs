using TemplateService.Domain.Enums;

namespace TemplateService.Application.Teams.Services;

public interface ITeamValidationService
{
    void ValidateName(string name);
    void ValidateDescription(string description);
    void ValidateDeletePermission(Guid userId, Guid teamOwnerId, UserRoleEnum role);
    void ValidateDeleteMemberPermission(Guid userThatDeletesId, Guid userToDeleteId, Guid teamOwnerId, UserRoleEnum userThatDeletesRoles);
    void ValidateAddToTeamPermission(Guid userId, Guid teamOwnerId, UserRoleEnum role);
}