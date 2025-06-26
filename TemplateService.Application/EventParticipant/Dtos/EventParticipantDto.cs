using System;
using TemplateService.Domain.Entities;
using TemplateService.Domain.Enums;


namespace TemplateService.Application.EventParticipant.Dtos;


public record EventParticipantDto (
    Guid UserId,
    Guid EventId,
    UserEntity User,
    EventEntity Event,
    bool IsSpeaker,
    bool AttendanceMarked);