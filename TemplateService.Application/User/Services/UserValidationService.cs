﻿using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using TemplateService.Domain.Enums;

namespace TemplateService.Application.User.Services;

public partial class UserValidationService : IUserValidationService
{
    
    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
    private static partial Regex EmailRegex();
    
    [GeneratedRegex(@"^@([a-zA-Z][a-zA-Z0-9_]{4,31})$", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
    private static partial Regex TelegramRegex();
    
    [GeneratedRegex(@"^[a-zA-Z0-9_!%$]+$", RegexOptions.Compiled)]
    private static partial Regex PasswordAllowedCharsRegex();

    private static readonly string[] AllowedMimeTypes = { "image/jpeg", "image/png", "image/webp" };
    private const long MaxFileSize = 2 * 1024 * 1024; // 2MB
    
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
        if (!EmailRegex().IsMatch(email))
            throw new ArgumentException("Email format is invalid.");
    }
    
    public void ValidateTelegramUsername(string telegram)
    {
        
        if (!TelegramRegex().IsMatch(telegram))
            throw new ArgumentException("Telegram format is invalid.");
    }

    public void ValidatePassword(string password)
    {
        if (string.IsNullOrEmpty(password) || password.Length < 6 || password.Length > 1024)
            throw new ArgumentException("Password must be between 6 and 1024 characters long.");
        if (!PasswordAllowedCharsRegex().IsMatch(password))
            throw new ArgumentException("Ivalid password characters, only \"^[a-zA-Z0-9_!%$]+$\" are allowed");
    }

    public void ValidateDeletePermissions(UserRoleEnum userRole)
    {
        if (userRole != UserRoleEnum.administrator)
            throw new UnauthorizedAccessException("Only administrator can delete users");
    }

    public void ValidateAvatar(IFormFile avatar)
    {
        if (avatar == null || avatar.Length == 0)
            throw new ArgumentException("Avatar is missing.");

        if (avatar.Length > MaxFileSize)
            throw new ArgumentException("Avatar size should be lower than 2MB.");

        if (!AllowedMimeTypes.Contains(avatar.ContentType))
            throw new ArgumentException("Forbidden avatar format. Allowed: JPEG, PNG, WEBP.");
    }
}