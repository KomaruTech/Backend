using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using TemplateService.Application.EventParticipant.Dtos;
using TemplateService.Domain.Entities;
using TemplateService.Infrastructure.Persistence;

namespace TemplateService.Application.EventParticipant.Queries;

internal class GetEventParticipantQueryHandler : IRequestHandler<GetEventParticipantQuery, EventParticipantDto>
{
    private readonly TemplateDbContext _dbContext;
    private readonly IMapper _mapper;

    public GetEventParticipantQueryHandler(TemplateDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<EventParticipantDto> Handle(GetEventParticipantQuery query, CancellationToken ct)
    {
        return await _dbContext.EventParticipants
            .Where(u => u.UserId == query.Id)
            .ProjectTo<EventParticipantDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(ct);
    }
}