﻿using TemplateService.Application.Event.DTOs;

namespace TemplateService.Application.Event.Commands;

public record InviteUserToEventCommand
(
    Guid EventId,
    Guid UserId,
    bool AsSpeaker = false
) : IRequest<EventDto>;