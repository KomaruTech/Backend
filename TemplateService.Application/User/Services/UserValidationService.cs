using System.Text.RegularExpressions;
using TemplateService.Domain.Enums;

namespace TemplateService.Application.User.Services;

public partial class UserValidationService : IUserValidationService
{
    public void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name) || name.Length < 2 || name.Length > 32)
            throw new ArgumentException("Name must be between 2 and 32 characters long.");
    }

    public void ValidateSurname(string surname)
    {
        if (string.IsNullOrWhiteSpace(surname) || surname.Length < 2 || surname.Length > 64)
            throw new ArgumentException("Surname must be between 2 and 64 characters long.");
    }

    public void ValidateEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email must not be empty.");

        if (!EmailRegex().IsMatch(email))
            throw new ArgumentException("Email format is invalid.");
    }

    public void ValidatePassword(string password)
    {
        if (string.IsNullOrEmpty(password) || password.Length < 6 || password.Length > 1024)
            throw new ArgumentException("Password must be between 6 and 1024 characters long.");
    }

    public void ValidateDeletePermissions(UserRoleEnum userRole)
    {
        if (userRole != UserRoleEnum.administrator)
            throw new UnauthorizedAccessException("Only administrator can delete users");
    }

    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
    private static partial Regex EmailRegex();
}