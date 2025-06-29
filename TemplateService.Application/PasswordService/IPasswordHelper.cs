namespace TemplateService.Application.PasswordService;
public interface IPasswordHelper
{
    string HashPassword(string password);
    bool VerifyPassword(string hash, string password);
    string GeneratePassword(int length = 16);
}
