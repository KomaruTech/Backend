using AutoMapper;
using TemplateService.Application.User.DTOs;
using TemplateService.Domain.Entities;

namespace TemplateService.Application.User.Services;

public interface IUserHelperService
{
    string GetAvatarUrl(Guid userId);
    bool IsAvatarExists(byte[]? avatar);
    UserDto BuildUserDto(UserEntity user, IMapper mapper);
}