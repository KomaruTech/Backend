using AutoMapper;
using TemplateService.Domain.Entities;
using TemplateService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using TemplateService.Application.PasswordService;
using TemplateService.Application.User.Services;

namespace TemplateService.Application.User.Commands;

internal class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, CreatedUserResult>
{
    private readonly TemplateDbContext _dbContext;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUserFieldValidationService _userFieldValidationService;

    public CreateUserCommandHandler(
        TemplateDbContext dbContext,
        IMapper mapper,
        IPasswordHasher passwordHasher,
        IUserFieldValidationService userFieldValidationService)
    {
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
        _userFieldValidationService = userFieldValidationService;
    }

    public async Task<CreatedUserResult> Handle(CreateUserCommand command, CancellationToken ct)
    {
        if (!_userFieldValidationService.IsValidName(command.Name))
            throw new ArgumentException("Invalid name, must be 2 <= name_length <= 32");

        if (!_userFieldValidationService.IsValidSurname(command.Surname))
            throw new ArgumentException("Invalid name, must be 2 <= surname_length <= 64");

        if (!_userFieldValidationService.IsValidEmail(command.Email))
            throw new ArgumentException("Email does not look like Email.");

        var password = _passwordHasher.GeneratePassword();

        // Генерируем общий ID для связки
        var id = Guid.NewGuid();

        var notificationPreferences = new NotificationPreferencesEntity
        {
            Id = id
            // Остальные поля не заполняем, они возьмутся из БД
        };

        // Генерация логина
        var baseLogin = GenerateBaseLogin(command.Name, command.Surname); // например, LeushkinM
        var finalLogin = await GenerateUniqueLoginAsync(baseLogin, ct); // LeushkinM, LeushkinM1, ...


        var user = new UserEntity
        {
            Id = id,
            Login = finalLogin,
            Name = command.Name,
            Surname = command.Surname,
            Email = command.Email,
            Role = command.Role,
            NotificationPreferencesId = id,
            NotificationPreferences = notificationPreferences,
            PasswordHash = _passwordHasher.HashPassword(password)
        };

        var resultedUser = new CreatedUserResult(
            Login: user.Login,
            Password: password
        );

        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync(ct);

        return resultedUser;
    }

    /// <summary>
    /// Создает логин из имени + фамилии пользователя
    /// </summary>
    private string GenerateBaseLogin(string name, string surname)
    {
        // Первая буква фамилии заглавная + остальная фамилия без изменений
        var surnameFormatted = char.ToUpper(surname[0]) + surname.Substring(1);

        var firstLetter = char.ToUpper(name[0]);

        return $"{surnameFormatted}{firstLetter}";
    }

    /// <summary>
    /// Создает уникальный логин, т.е добавляет к логину, например LeushkinM цифру при повторении
    /// </summary>
    private async Task<string> GenerateUniqueLoginAsync(string baseLogin, CancellationToken ct)
    {
        // Получаем все логины, начинающиеся с baseLogin, например "LeushkinM", "LeushkinM1", "LeushkinM2"
        var existingLogins = await _dbContext.Users
            .Where(u => EF.Functions.Like(u.Login, baseLogin + "%"))
            .Select(u => u.Login)
            .ToListAsync(ct);

        if (!existingLogins.Contains(baseLogin))
            return baseLogin;

        // Фильтруем логины, которые соответствуют шаблону: baseLogin + число
        var maxSuffix = existingLogins
            .Select(login => login.Substring(baseLogin.Length)) // всё после baseLogin
            .Where(suffix => int.TryParse(suffix, out _)) // оставляем только числа
            .Select(int.Parse)
            .DefaultIfEmpty(0) // если ничего нет — 0
            .Max();

        return $"{baseLogin}{maxSuffix + 1}"; // Возвращаем Логин с +1
    }
}