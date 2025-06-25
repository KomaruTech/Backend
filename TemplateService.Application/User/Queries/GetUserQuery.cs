using MediatR;
using TemplateService.Application.User.DTOs;

namespace TemplateService.Application.User.Queries;

public record GetUserQuery(Guid Id) : IRequest<UserDto>;