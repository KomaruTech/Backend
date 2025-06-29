using TemplateService.Domain.Enums;

namespace TemplateService.Application.User.Services
{
    public interface IUserValidationService
    {
        void ValidateName(string name);
        void ValidateSurname(string surname);
        void ValidateEmail(string email);
        void ValidatePassword(string password);
        void ValidateDeletePermissions(UserRoleEnum userRole);
    }
}