
using TemplateService.Domain.Entities;

namespace TemplateService.Application.EventPhotos.Dtos;

public record EventPhotosDto (
    Guid Id,
    Guid EventId,
    EventEntity Event,
    byte[] Image ,
    string MimeType,
    DateTime UploadedAt,
    string? Description); 
