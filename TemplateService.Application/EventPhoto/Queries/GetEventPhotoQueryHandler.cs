using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using TemplateService.Application.EventPhoto.Dtos;
using TemplateService.Domain.Entities;
using TemplateService.Infrastructure.Persistence;

namespace TemplateService.Application.EventPhoto.Queries;

internal class GetEventPhotoQueryHandler : IRequestHandler<GetEventPhotoQuery, EventPhotoDto>
{
    private readonly TemplateDbContext _dbContext;
    private readonly IMapper _mapper;

    public GetEventPhotoQueryHandler(TemplateDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<EventPhotoDto> Handle(GetEventPhotoQuery query, CancellationToken ct)
    {
        return await _dbContext.EventPhoto
            .Where(u => u.Id == query.Id)
            .ProjectTo<EventPhotoDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(ct);
    }
}