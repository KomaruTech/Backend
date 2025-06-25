// using AutoMapper;
// using TemplateService.Application.Document.Dtos;
// using TemplateService.Domain.Entities;
//
// namespace TemplateService.Application;
//
// public class MappingProfile : Profile
// {
//     public MappingProfile()
//     {
//         CreateMap<DocumentEntity, DocumentDto>();
//         CreateMap<MetaEntity, MetaDto>()
//             .ForMember(p => p.MetaTypeName, o => o.MapFrom(s => s.MetaTypeId.ToString()));
//     }
// }
