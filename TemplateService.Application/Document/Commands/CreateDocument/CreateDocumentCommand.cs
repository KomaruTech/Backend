using TemplateService.Application.Document.Dtos;

namespace TemplateService.Application.Document.Commands.CreateDocument;

public class CreateDocumentCommand : IRequest<DocumentDto>
{
    public CreateDocumentCommand(string documentName, Guid? userId = null, Guid? documentTypeId = null, int? documentNumber = null)
    {
        UserId = userId;
        DocumentTypeId = documentTypeId;
        DocumentNumber = documentNumber;
        DocumentName = documentName;
    }

    public Guid? UserId { get; }
    public Guid? DocumentTypeId { get; }
    public int? DocumentNumber { get; }
    public string DocumentName { get; }   
}
