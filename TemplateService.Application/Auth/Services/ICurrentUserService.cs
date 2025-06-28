using TemplateService.Domain.Enums;

namespace TemplateService.Application.Auth.Services;

public interface ICurrentUserService
{
    Guid GetUserId();
    UserRoleEnum GetUserRole();
}