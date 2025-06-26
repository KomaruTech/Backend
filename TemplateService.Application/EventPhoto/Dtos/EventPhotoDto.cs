using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateService.Domain.Entities;

namespace TemplateService.Application.EventPhoto.Dtos;

public record EventPhotoDto (
    Guid Id,
    Guid EventId,
    EventEntity Event,
    byte[] Image ,
    string MimeType,
    DateTime UploadedAt,
    string? Description); 
