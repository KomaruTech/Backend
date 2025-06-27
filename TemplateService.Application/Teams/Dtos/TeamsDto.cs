using System;
using TemplateService.Domain.Entities;
using TemplateService.Domain.Enums;

namespace TemplateService.Application.Teams.Dtos;

public record TeamsDto(
    Guid Id,
    string Name,
    string Description,
    ICollection<UserBriefDto> Users); // Заменили UserTeamsEntity на UserBriefDto

public record UserBriefDto(
    Guid Id,
    string Name,
    string Email);
