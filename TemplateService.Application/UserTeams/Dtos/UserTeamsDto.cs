using System;
using TemplateService.Domain.Entities;
using TemplateService.Domain.Enums;

namespace TemplateService.Application.UserTeams.Dtos;

public record UserTeamsDto(
    Guid UserId,
    Guid TeamId,
    UserEntity User,
    TeamsEntity Team);
