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
                opt => opt.MapFrom(src => src.EventTeams.Select(et => et.TeamId).ToList()));
        ;

        CreateMap<UserEntity, UserDto>()
            .ForMember(dest => dest.AvatarUrl, opt =>
                opt.MapFrom<AvatarUrlResolver>());

        CreateMap<CreateUserCommand, UserEntity>()
            .ForMember(dest => dest.Id, opt => opt.Ignore());

        CreateMap<NotificationPreferencesEntity, UserNotificationPreferencesDto>()
            .ForMember(dest => dest.NotifyBeforeOneDay, opt => opt.MapFrom(src => src.ReminderBefore1Day))
            .ForMember(dest => dest.NotifyBeforeOneHour, opt => opt.MapFrom(src => src.ReminderBefore1Hour))
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.NotifyTelegram, opt => opt.MapFrom(src => src.NotifyTelegram));

        CreateMap<UpdateEventCommand, EventEntity>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        CreateMap<UpdateUserProfileCommand, UserEntity>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        CreateMap<UserTeamsEntity, UserDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.User.Id))
            .ForMember(dest => dest.Login, opt => opt.MapFrom(src => src.User.Login))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.User.Name))
            .ForMember(dest => dest.Surname, opt => opt.MapFrom(src => src.User.Surname))
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.User.Role))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email))
            .ForMember(dest => dest.TelegramUsername, opt => opt.MapFrom(src => src.User.TelegramUsername));

        CreateMap<TeamsEntity, TeamsDto>()
            .ForMember(dest => dest.Users, opt => opt
                .MapFrom(src => src.Users.Select(ut => ut.User)));
    }
}