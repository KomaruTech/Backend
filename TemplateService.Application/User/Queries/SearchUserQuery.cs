using TemplateService.Application.User.DTOs;
using TemplateService.Domain.Enums;

namespace TemplateService.Application.User.Queries;

public record SearchUserQuery(
    string Query,
    UserRoleEnum? Role
) : IRequest<List<UserDto>>;