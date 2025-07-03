using AutoMapper;
using TemplateService.Domain.Entities;
using TemplateService.Application.User.DTOs;

namespace TemplateService.Application.User.Services;

public class AvatarUrlResolver : IValueResolver<UserEntity, UserDto, string?>
{
    private readonly IUserHelperService _userHelper;

    public AvatarUrlResolver(IUserHelperService userHelper)
    {
        _userHelper = userHelper;
    }

    public string? Resolve(UserEntity source, UserDto destination, string? destMember, ResolutionContext context)
    {
        return _userHelper.IsAvatarExists(source.Avatar)
            ? _userHelper.GetAvatarUrl(source.Id)
            : null;
    }
}