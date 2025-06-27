using MediatR;
using TemplateService.Application.EventParticipant.Dtos;

namespace TemplateService.Application.EventParticipant.Queries;

public record GetEventParticipantQuery(Guid Id) : IRequest<EventParticipantDto>;