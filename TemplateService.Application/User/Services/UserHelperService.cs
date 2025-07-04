using AutoMapper;
using TemplateService.Application.User.DTOs;
using TemplateService.Domain.Entities;

namespace TemplateService.Application.User.Services;

public class UserHelperService: IUserHelperService
{
    public string GetAvatarUrl(Guid userId)
    {
        return $"/user/{userId}/avatar";
    }

    public bool IsAvatarExists(byte[]? avatar)
    {
        return avatar != null && avatar.Length > 0;
    }
    
    public UserDto BuildUserDto(UserEntity user, IMapper mapper)
    {
        var dto = mapper.Map<UserDto>(user);
        dto.AvatarUrl = IsAvatarExists(user.Avatar) ? GetAvatarUrl(user.Id) : null;
        return dto;
    }

}