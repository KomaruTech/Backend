using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using TemplateService.Application.EventParticipant.Queries;
using TemplateService.Application.SpeakerApplication.Dtos;
using TemplateService.Domain.Entities;
using TemplateService.Infrastructure.Persistence;

namespace TemplateService.Application.SpeakerApplication.Queries;

internal class GetSpeakerApplicationQueryHandler : IRequestHandler<GetSprakerApplicationsQuery, SpeakerApplicationsDto>
{
    private readonly TemplateDbContext _dbContext;
    private readonly IMapper _mapper;

    public GetSpeakerApplicationQueryHandler(TemplateDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<SpeakerApplicationsDto> Handle(GetSprakerApplicationsQuery query, CancellationToken ct)
    {
        return await _dbContext.EventParticipants
            .Where(u => u.UserId == query.Id)
            .ProjectTo<SpeakerApplicationsDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(ct);
    }
}