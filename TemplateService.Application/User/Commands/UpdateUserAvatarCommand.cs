using Microsoft.AspNetCore.Http;

namespace TemplateService.Application.User.Commands;

public record UpdateUserAvatarCommand(IFormFile Avatar) : IRequest<UpdatedUserAvatarResult>;