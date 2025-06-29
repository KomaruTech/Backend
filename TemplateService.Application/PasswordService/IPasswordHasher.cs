namespace TemplateService.Application.PasswordService;
public interface IPasswordHasher
{
    string HashPassword(string password);
    bool VerifyPassword(string hash, string password);
    string GeneratePassword(int length = 16);
}
