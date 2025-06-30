using TemplateService.Domain.Enums;

namespace TemplateService.Application.Event.Services;

public class EventValidationService : IEventValidationService
{
    public void ValidateName(string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name must be at least 4 characters long.");
        if (name.Length < 4 || name.Length > 64)
            throw new ArgumentException("Name must be between 4 and 64 characters long.");
    }

    public void ValidateDescription(string? description)
    {
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Description must be at least 16 characters long.");
        if (description.Length < 16 || description.Length > 10000)
            throw new ArgumentException("Description must be between 16 and 10000 characters long.");
    }

    public void ValidateLocation(string? location)
    {
        if (string.IsNullOrWhiteSpace(location) || location.Length > 1000)
            throw new ArgumentException("Location must not exceed 1000 characters.");
    }

    public void ValidateUserRole(UserRoleEnum userRole)
    {
        if (userRole == UserRoleEnum.member)
            throw new InvalidOperationException("User with role 'member' can't create events.");
    }

    public void ValidateTimeStart(DateTime timeStart)
    {
        if (timeStart < DateTime.UtcNow.AddHours(2))
            throw new ArgumentException("The event must start at least 2 hours from now.");
    }

    public void ValidateDuration(DateTime timeStart, DateTime? timeEnd)
    {
        if (timeEnd.HasValue)
        {
            var duration = timeEnd.Value - timeStart;
            if (duration < TimeSpan.FromMinutes(10) || duration > TimeSpan.FromHours(24))
                throw new ArgumentException("Event duration must be between 10 minutes and 24 hours.");
        }
    }

    public void ValidateUpdatePermissions(Guid userId, Guid createdById, UserRoleEnum userRole)
    {
        if (userId != createdById && userRole != UserRoleEnum.administrator)
            throw new UnauthorizedAccessException("You don't have permission to edit this event.");
    }

    public void ValidateConfirmPermissions(Guid userId, Guid createdById, UserRoleEnum userRole)
    {
        if (userId != createdById && userRole == UserRoleEnum.member)
            throw new UnauthorizedAccessException("You don't have permission to confirm this event.");
    }
}