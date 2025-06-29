using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using TemplateService.Application.User.DTOs;
using TemplateService.Application.User.Services;
using TemplateService.Infrastructure.Persistence;

namespace TemplateService.Application.User.Queries;

internal class GetUserQueryHandler : IRequestHandler<GetUserQuery, UserDto>
{
    private readonly TemplateDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly IUserHelperService _userHelperService;

    public GetUserQueryHandler(
        TemplateDbContext dbContext,
        IMapper mapper,
        IUserHelperService userHelperService
    )
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _userHelperService = userHelperService;
    }

    public async Task<UserDto> Handle(GetUserQuery query, CancellationToken cancellationToken)
    {
        var user = await _dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Login == query.Login, cancellationToken);

        return user == null ? null : _userHelperService.BuildUserDto(user, _mapper);
    }
}