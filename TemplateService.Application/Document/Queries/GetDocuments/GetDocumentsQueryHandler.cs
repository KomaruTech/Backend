// using AutoMapper;
// using AutoMapper.QueryableExtensions;
// using Microsoft.EntityFrameworkCore;
// using TemplateService.Application.Document.Dtos;
// using TemplateService.Infrastructure.Persistence;
//
// namespace TemplateService.Application.Document.Queries.GetDocuments;
//
// internal class GetDocumentsQueryHandler : IRequestHandler<GetDocumentsQuery, IEnumerable<DocumentDto>>
// {
//     private readonly TemplateDbContext _dbContext;
//     private readonly IMapper _mapper;
//
//     public GetDocumentsQueryHandler(TemplateDbContext dbContext, IMapper mapper)
//     {
//         _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
//         _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
//     }
//
//     public async Task<IEnumerable<DocumentDto>> Handle(GetDocumentsQuery query, CancellationToken cancellationToken)
//     {
//         var q = string.IsNullOrEmpty(query.Name) ? _dbContext.Documents.AsQueryable() : _dbContext.Documents.Where(x=>x.Name.ToUpper().Contains(query.Name.ToUpper()));
//         return await q.Skip(query.Skip).Take(query.Take).OrderBy(x=>x.CreationDate).ProjectTo<DocumentDto>(_mapper.ConfigurationProvider).ToListAsync(cancellationToken);
//     }
// }
