using TemplateService.Application.Document.Dtos;

namespace TemplateService.Application.Document.Queries.GetDocuments;

public class GetDocumentsQuery : IRequest<IEnumerable<DocumentDto>>
{
    public GetDocumentsQuery(string name = null, int skip = 0, int take = 20)
    {
        Name = name;
        Skip = skip;
        Take = take;
    }

    public string Name { get; }
    public int Skip { get; }
    public int Take { get; }

}
