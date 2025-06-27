using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using TemplateService.Application.EventParticipant.Queries;
using TemplateService.Application.SpeakerApplication.Dtos;
using TemplateService.Application.Teams.Dtos;
using TemplateService.Domain.Entities;
using TemplateService.Infrastructure.Persistence;

namespace TemplateService.Application.SpeakerApplication.Queries;

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
        return await _dbContext.Teams
            .Where(u => u.Id == query.Id)
            .ProjectTo<TeamsDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(ct);
    }
}