using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using TemplateService.Application.User.DTOs;
using TemplateService.Infrastructure.Persistence;

namespace TemplateService.Application.User.Queries;

internal class GetUserQueryHandler : IRequestHandler<GetUserQuery, UserDto>
{
    private readonly TemplateDbContext _dbContext;
    private readonly IMapper _mapper;

    public GetUserQueryHandler(TemplateDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<UserDto> Handle(GetUserQuery query, CancellationToken ct)
    {
        return await _dbContext.Users
            .Where(u => u.Login == query.Login)
            .ProjectTo<UserDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(ct);
    }
}