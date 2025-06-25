using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using TemplateService.Application.Event.DTOs;
using TemplateService.Domain.Entities;
using TemplateService.Infrastructure.Persistence;

namespace TemplateService.Application.Event.Queries.GetEvent;

internal class GetEventQueryHandler : IRequestHandler<GetEventQuery, EventDto>
{
    private readonly TemplateDbContext _dbContext;
    private readonly IMapper _mapper;

    public GetEventQueryHandler(TemplateDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<EventDto> Handle(GetEventQuery query, CancellationToken ct)
    {
        return await _dbContext.Users
            .Where(u => u.Id == query.Id)
            .ProjectTo<EventDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(ct);
    }
}