//using AutoMapper;
//using InformService.Atach.WebUI.Proto.Document;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Configuration;
//using TemplateService.Application.Document.Dtos;
//using TemplateService.Domain.Entities;
//using TemplateService.Domain.Enums;
//using TemplateService.Infrastructure.Persistence;

//namespace TemplateService.Application.Document.Commands.CreateDocument;

//internal class CreateDocumentCommandHandler : IRequestHandler<CreateDocumentCommand, DocumentDto>
//{
//    private readonly DocumentGrpcService.DocumentGrpcServiceClient _documentGrpcService;
//    private readonly TemplateDbContext _dbContext;
//    private readonly IMapper _mapper;
//    private readonly IConfiguration _configuration;

//    public CreateDocumentCommandHandler(DocumentGrpcService.DocumentGrpcServiceClient documentGrpcService, TemplateDbContext dbContext, IMapper mapper, IConfiguration configuration)
//    {
//        _documentGrpcService = documentGrpcService ?? throw new ArgumentNullException(nameof(documentGrpcService));
//        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
//        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
//        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
//    }

//    public async Task<DocumentDto> Handle(CreateDocumentCommand command, CancellationToken cancellationToken)
//    {
//        var num = command.DocumentNumber ?? (((await _dbContext.Documents.MaxAsync(x => (int?)x.Number, cancellationToken: cancellationToken)) ?? 0) + 1);
//        var docTypeId = command.DocumentTypeId ?? _configuration.GetValue<Guid>("DefaultDocumentTypeId");
//        var userId = command.UserId ?? _configuration.GetValue<Guid>("DefaultUserId");

//        var newDoc = new DocumentEntity
//        {
//            Id = Guid.NewGuid(),
//            Number = num,
//            Name = command.DocumentName,
//            CreationDate = DateTime.UtcNow
//        };

//        var resp = await _documentGrpcService.CreateDocumentAsync(new CreateDocumentRequest
//        {
//            DocumentTypeId = docTypeId.ToString(),
//            ExternalId = newDoc.Id.ToString(),
//            UserId = userId.ToString()
//        }, cancellationToken: cancellationToken);

//        if (resp.Success == true)
//        {
//            newDoc.Metas.Add(new MetaEntity
//            {
//                MetaTypeId = MetaTypeEnum.AtachDocumentId,
//                Data = resp.DocumentId.ToString()
//            });

//            _dbContext.Documents.Add(newDoc);
//            await _dbContext.SaveChangesAsync(cancellationToken);
//            return _mapper.Map<DocumentDto>(newDoc);
//        }

//        throw new Exception(resp.ErrorMessage);
//    }
//}
