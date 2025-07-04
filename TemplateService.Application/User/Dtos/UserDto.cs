using TemplateService.Domain.Enums;

namespace TemplateService.Application.User.DTOs;

public class UserDto
{
    public Guid Id { get; set; }
    public string Login { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public UserRoleEnum Role { get; set; }
    public string Email { get; set; }
    public string? TelegramUsername { get; set; }
    public string? AvatarUrl { get; set; }
    public UserDto() {}
}