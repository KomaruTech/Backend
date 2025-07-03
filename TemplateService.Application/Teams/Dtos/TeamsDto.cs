using System;using TemplateService.Application.User.DTOs;
using TemplateService.Domain.Entities;
using TemplateService.Domain.Enums;

namespace TemplateService.Application.Teams.Dtos;

public record TeamsDto(
    Guid Id,
    string Name,
    string Description,
    Guid OwnerId,
    ICollection<UserDto> Users);
