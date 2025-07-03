using AutoMapper;
using TemplateService.Application.Event.Commands;
using TemplateService.Application.Event.DTOs;
using TemplateService.Application.Teams.Dtos;
using TemplateService.Application.User.Commands;
using TemplateService.Application.User.Dtos;
using TemplateService.Application.User.DTOs;
using TemplateService.Application.User.Services;
using TemplateService.Domain.Entities;

namespace TemplateService.Application;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<EventEntity, EventDto>()
            .ForMember(dest => dest.ParticipantIds,
                opt => opt.MapFrom(src => src.Participants.Select(p => p.UserId).ToList()))
            .ForMember(dest => dest.TeamIds,
                opt => opt.MapFrom(src => src.EventTeams.Select(et => et.TeamId).ToList()));;
        
        CreateMap<UserEntity, UserDto>()
            .ForMember(dest => dest.AvatarUrl, opt =>
                opt.MapFrom<AvatarUrlResolver>());
        
        CreateMap<CreateUserCommand, UserEntity>()
            .ForMember(dest => dest.Id, opt => opt.Ignore());

        CreateMap<UserNotificationPreferencesDto, NotificationPreferencesEntity>();
        
        CreateMap<UpdateEventCommand, EventEntity>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        
        CreateMap<UpdateUserProfileCommand, UserEntity>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        
        CreateMap<UserTeamsEntity, UserDto>()
            .ConvertUsing((ut, dest, context) =>
            {
                if (ut.User == null)
                    return null!;

                var userDto = new UserDto(
                    ut.User.Id,
                    ut.User.Login,
                    ut.User.Name,
                    ut.User.Surname,
                    ut.User.Role,
                    ut.User.Email,
                    ut.User.TelegramUsername
                );

                // Теперь заполняем AvatarUrl через маппер
                userDto.AvatarUrl = context.Mapper.Map<UserDto>(ut.User).AvatarUrl;

                return userDto;
            });
        // Маппинг TeamsEntity -> TeamsDto
        CreateMap<TeamsEntity, TeamsDto>()
            .ForMember(dest => dest.Users, opt => opt
                .MapFrom(src => src.Users.Select(ut => ut.User)));         // из ICollection<UserTeamsEntity> берем UserEntity и маппим в UserDto
    }
}