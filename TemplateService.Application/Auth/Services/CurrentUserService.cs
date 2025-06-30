using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using TemplateService.Domain.Enums;

namespace TemplateService.Application.Auth.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid GetUserId()
    {
        var user = _httpContextAccessor.HttpContext?.User;

        var idClaim = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!Guid.TryParse(idClaim, out var userId))
            throw new UnauthorizedAccessException("Invalid UserID in token");

        return userId;
    }

    public UserRoleEnum GetUserRole()
    {
        var user = _httpContextAccessor.HttpContext?.User;

        var roleClaim = user?.FindFirst(ClaimTypes.Role)?.Value;

        if (string.IsNullOrWhiteSpace(roleClaim))
            throw new UnauthorizedAccessException("Role claim is missing in token");

        if (!Enum.TryParse<UserRoleEnum>(roleClaim, ignoreCase: true, out var role))
            throw new UnauthorizedAccessException("Invalid role in token");

        return role;
    }
}