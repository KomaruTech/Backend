using TemplateService.Application.Document.Dtos;

namespace TemplateService.Application.Document.Queries.GetDocument;

public class GetDocumentQuery : IRequest<DocumentDto>
{
    public GetDocumentQuery(Guid id)
    {
        Id = id;
    }

    public Guid Id { get; }
}
