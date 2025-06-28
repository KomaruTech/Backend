using TemplateService.Application.User.DTOs;

namespace TemplateService.Application.Auth.Commands;

public record LoginUserResult(UserDto User, string Token);