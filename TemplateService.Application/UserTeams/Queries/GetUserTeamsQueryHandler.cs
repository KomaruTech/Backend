using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using TemplateService.Application.UserTeams.Dtos;
using TemplateService.Infrastructure.Persistence;

namespace TemplateService.Application.UserTeams.Queries.GetUser;

internal class GetUserTeamsQueryHandler : IRequestHandler<GetUserTeamsQuery, UserTeamsDto>
{
    private readonly TemplateDbContext _dbContext;
    private readonly IMapper _mapper;

    public GetUserTeamsQueryHandler(TemplateDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<UserTeamsDto> Handle(GetUserTeamsQuery query, CancellationToken ct)
    {
        return await _dbContext.Users
            .Where(u => u.Id == query.Id)
            .ProjectTo<UserTeamsDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(ct);
    }
}