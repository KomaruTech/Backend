using TemplateService.Domain.Enums;

namespace TemplateService.Application.Event.DTOs;

public class EventDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public DateTime TimeStart { get; set; }
    public DateTime? TimeEnd { get; set; }
    public EventTypeEnum Type { get; set; }
    public EventStatusEnum Status { get; set; }
    public string? Location { get; set; }
    public Guid CreatedById { get; set; }
    public List<string> Keywords { get; set; } = new();
    public List<Guid> ParticipantIds { get; set; } = new();
    public List<Guid> TeamIds { get; set; } = new();
}