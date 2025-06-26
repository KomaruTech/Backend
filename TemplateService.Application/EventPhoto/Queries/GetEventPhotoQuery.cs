using MediatR;
using TemplateService.Application.EventPhoto.Dtos;

namespace TemplateService.Application.EventPhoto.Queries;

public record GetEventPhotoQuery(Guid Id) : IRequest<EventPhotoDto>;