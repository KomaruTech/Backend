using TemplateService.Application.User.DTOs;


namespace TemplateService.Application.Teams.Dtos;

public record TeamsDto(
    Guid Id,
    string Name,
    string Description,
    Guid OwnerId,
    ICollection<UserDto> Users);
