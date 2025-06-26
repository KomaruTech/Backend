using MediatR;
using TemplateService.Application.SpeakerApplication.Dtos;

namespace TemplateService.Application.EventParticipant.Queries;

public record GetSprakerApplicationsQuery(Guid Id) : IRequest<SpeakerApplicationsDto>;