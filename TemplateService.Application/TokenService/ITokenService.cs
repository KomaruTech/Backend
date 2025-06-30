using TemplateService.Application.User.DTOs;

namespace TemplateService.Application.TokenService;

public interface ITokenService
{
    string CreateToken(UserDto user);
}