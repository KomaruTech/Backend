using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using TemplateService.Application.Document.Dtos;
using TemplateService.Infrastructure.Persistence;

namespace TemplateService.Application.Document.Queries.GetDocument;

internal class GetDocumentQueryHandler : IRequestHandler<GetDocumentQuery, DocumentDto>
{
    private readonly TemplateDbContext _dbContext;
    private readonly IMapper _mapper;

    public GetDocumentQueryHandler(TemplateDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<DocumentDto> Handle(GetDocumentQuery query, CancellationToken cancellationToken)
    {
        return await _dbContext.Documents.ProjectTo<DocumentDto>(_mapper.ConfigurationProvider).FirstOrDefaultAsync(x => x.Id == query.Id, cancellationToken);
    }
}
