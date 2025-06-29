namespace TemplateService.Application.User.Queries;

public record GetUserAvatarQuery(Guid UserId) : IRequest<UserAvatarResult>;