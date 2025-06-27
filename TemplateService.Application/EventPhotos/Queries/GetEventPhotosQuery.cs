using MediatR;
using TemplateService.Application.EventPhotos.Dtos;

namespace TemplateService.Application.EventPhotos.Queries;

public record GetEventPhotosQuery(Guid Id) : IRequest<EventPhotosDto>;