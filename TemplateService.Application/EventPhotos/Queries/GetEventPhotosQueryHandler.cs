using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using TemplateService.Application.EventPhotos.Dtos;
using TemplateService.Infrastructure.Persistence;

namespace TemplateService.Application.EventPhotos.Queries;

internal class GetEventPhotosQueryHandler : IRequestHandler<GetEventPhotosQuery, EventPhotosDto>
{
    private readonly TemplateDbContext _dbContext;
    private readonly IMapper _mapper;

    public GetEventPhotosQueryHandler(TemplateDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<EventPhotosDto> Handle(GetEventPhotosQuery query, CancellationToken ct)
    {
        return await _dbContext.EventPhotos
            .Where(u => u.Id == query.Id)
            .ProjectTo<EventPhotosDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(ct);
    }
}