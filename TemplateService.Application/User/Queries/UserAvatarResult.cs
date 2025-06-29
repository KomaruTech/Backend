namespace TemplateService.Application.User.Queries;

public record UserAvatarResult(
    byte[] AvatarBytes,
    string MimeType
);