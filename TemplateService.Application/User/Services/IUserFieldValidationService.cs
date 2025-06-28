namespace TemplateService.Application.User.Services
{
    public interface IUserFieldValidationService
    {
        bool IsValidName(string name);
        bool IsValidSurname(string surname);
        bool IsValidEmail(string email);
        bool IsValidPassword(string password);
    }
}