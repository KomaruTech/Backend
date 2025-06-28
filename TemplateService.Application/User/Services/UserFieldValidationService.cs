using System.Text.RegularExpressions;

namespace TemplateService.Application.User.Services
{
    public class UserFieldValidationService : IUserFieldValidationService
    {
        public bool IsValidName(string name)
        {
            return !string.IsNullOrWhiteSpace(name) && name.Length >= 2 && name.Length <= 32;
        }

        public bool IsValidSurname(string surname)
        {
            return !string.IsNullOrWhiteSpace(surname) && surname.Length >= 2 && surname.Length <= 64;
        }

        public bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;

            // regex для валидации email
            var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
            return emailRegex.IsMatch(email);
        }

        public bool IsValidPassword(string password)
        {
            return !string.IsNullOrEmpty(password) && password.Length >= 6 && password.Length <= 1024;
        }
    }
}