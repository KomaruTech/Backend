using AutoMapper;
using TemplateService.Application.Event.Commands;
using TemplateService.Application.Event.DTOs;
using TemplateService.Application.Teams.Dtos;
using TemplateService.Application.User.Commands;
using TemplateService.Application.User.DTOs;
using TemplateService.Domain.Entities;

namespace TemplateService.Application;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<EventEntity, EventDto>()
            .ForMember(dest => dest.ParticipantIds,
                opt => opt.MapFrom(src => src.Participants.Select(p => p.UserId).ToList()));
        CreateMap<UserEntity, UserDto>()
            .ConstructUsing(u => new UserDto( 
                u.Id,
                u.Login,
                u.Name,
                u.Surname,
                u.Role,
                u.Email,
                u.TelegramUsername,
                null 
            ));
        
        CreateMap<CreateUserCommand, UserEntity>()
            .ForMember(dest => dest.Id, opt => opt.Ignore());
        
        CreateMap<UpdateEventCommand, EventEntity>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        
        CreateMap<UpdateUserProfileCommand, UserEntity>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        
        CreateMap<UserTeamsEntity, UserDto>()
            .ConvertUsing(ut => ut.User == null 
                ? null! 
                : new UserDto(
                    ut.User.Id,
                    ut.User.Login,
                    ut.User.Name,
                    ut.User.Surname,
                    ut.User.Role,
                    ut.User.Email,
                    ut.User.TelegramUsername,
                    null
                ));
        
        // Маппинг TeamsEntity -> TeamsDto
        CreateMap<TeamsEntity, TeamsDto>()
            .ForMember(dest => dest.Users, opt => opt
                .MapFrom(src => src.Users.Select(ut => ut.User)));         // из ICollection<UserTeamsEntity> берем UserEntity и маппим в UserDto
    }
}