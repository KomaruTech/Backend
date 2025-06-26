using System;
using TemplateService.Domain.Entities;
using TemplateService.Domain.Enums;


namespace TemplateService.Application.SpeakerApplication.Dtos;

public record SpeakerApplicationsDto (
    Guid Id,
    Guid UserId ,
    Guid EventId,
    UserEntity User,
    EventEntity Event,
    string Topic,
    DateTime CreatedAt,
    string? Comment,
     ApplicationStatusEnum Status);

