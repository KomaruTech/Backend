using TemplateService.Domain.Enums;

namespace TemplateService.Application.Teams.Services;

public class TeamValidationService : ITeamValidationService
{
    public void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name) || name.Length < 4 || name.Length > 64)
            throw new ArgumentException("Name length must be between 4 and 64 characters");
    }

    public void ValidateDescription(string description)
    {
        if (string.IsNullOrWhiteSpace(description) || description.Length < 10 || description.Length > 1000)
            throw new ArgumentException("Description length must be between 10 and 1000 characters");
    }
    
    public void ValidateDeletePermission(Guid userId, Guid teamOwnerId, UserRoleEnum role)
    {
        if (userId != teamOwnerId || role != UserRoleEnum.administrator)
            throw new ArgumentException("You don't have permission to delete this team");
    }
    
    public void ValidateAddToTeamPermission(Guid userId, Guid teamOwnerId, UserRoleEnum role)
    {
        if (userId != teamOwnerId || role != UserRoleEnum.administrator)
            throw new ArgumentException("You don't have permission to add users to this team");
    }

    public void ValidateDeleteMemberPermission(Guid userThatDeletesId,Guid userToDeleteId, Guid teamOwnerId, UserRoleEnum userThatDeletesRoles)
    {
        if (userToDeleteId == teamOwnerId)
            throw new ArgumentException("You can't delete owner of the team");
        
        if (userThatDeletesId != teamOwnerId || userThatDeletesRoles != UserRoleEnum.administrator)
            throw new ArgumentException("You don't have permission to delete members of the team");
    }
}