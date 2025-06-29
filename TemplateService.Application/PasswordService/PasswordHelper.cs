using System.Security.Cryptography;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace TemplateService.Application.PasswordService;

public partial class PasswordHelper : IPasswordHelper
{
    private static readonly char[] AllowedChars =
        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789_!%$".ToCharArray();
    
    public string HashPassword(string password)
    {
        byte[] salt = RandomNumberGenerator.GetBytes(16);
        
        byte[] hash = KeyDerivation.Pbkdf2(
            password: password,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 100_000,
            numBytesRequested: 32);

        var result = Convert.ToBase64String(salt) + "." + Convert.ToBase64String(hash);
        return result;
    }

    public bool VerifyPassword(string hash, string password)
    {
        var parts = hash.Split('.');
        if (parts.Length != 2)
            return false;

        var salt = Convert.FromBase64String(parts[0]);
        var expectedHash = Convert.FromBase64String(parts[1]);

        var actualHash = KeyDerivation.Pbkdf2(
            password: password,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 100_000,
            numBytesRequested: 32);

        return CryptographicOperations.FixedTimeEquals(expectedHash, actualHash);
    }
    public string GeneratePassword(int length = 16)
    {
        if (length <= 0) throw new ArgumentException("Length must be positive", nameof(length));

        var result = new char[length];
        using (var rng = RandomNumberGenerator.Create())
        {
            var buffer = new byte[sizeof(uint)];

            for (int i = 0; i < length; i++)
            {
                rng.GetBytes(buffer);
                uint num = BitConverter.ToUInt32(buffer, 0);
                result[i] = AllowedChars[num % AllowedChars.Length];
            }
        }

        return new string(result);
    }
    
}