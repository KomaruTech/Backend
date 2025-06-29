using AutoMapper;
using TemplateService.Application.Event.Commands;
using TemplateService.Application.Event.DTOs;
using TemplateService.Application.User.Commands;
using TemplateService.Application.User.DTOs;
using TemplateService.Domain.Entities;

namespace TemplateService.Application;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<EventEntity, EventDto>();
        CreateMap<UserEntity, UserDto>()
            .ConstructUsing(u => new UserDto(
                u.Id,
                u.Login,
                u.Name,
                u.Surname,
                u.Role,
                u.Email,
                u.TelegramId,
                null 
            ));
        
        CreateMap<CreateUserCommand, UserEntity>()
            .ForMember(dest => dest.Id, opt => opt.Ignore());
        
        CreateMap<UpdateEventCommand, EventEntity>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        
        CreateMap<UpdateUserProfileCommand, UserEntity>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
    }
}