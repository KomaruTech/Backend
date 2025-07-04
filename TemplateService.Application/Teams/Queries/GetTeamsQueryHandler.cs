using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using TemplateService.Application.Teams.Dtos;
using TemplateService.Infrastructure.Persistence;

namespace TemplateService.Application.Teams.Queries;

internal class GetTeamsQueryHandler : IRequestHandler<GetTeamsQuery, TeamsDto>
{
    private readonly TemplateDbContext _dbContext;
    private readonly IMapper _mapper;

    public GetTeamsQueryHandler(TemplateDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<TeamsDto> Handle(GetTeamsQuery query, CancellationToken ct)
    {
        var dto = await _dbContext.Teams
            .Include(t => t.Users)
            .ThenInclude(ut => ut.User)
            .Where(t => t.Id == query.Id)
            .ProjectTo<TeamsDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(ct);

        if (dto == null)
            throw new KeyNotFoundException($"Team with id {query.Id} not found");

        return dto;
    }
}