using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TemplateService.Application.PasswordService;
using TemplateService.Application.TokenService;
using TemplateService.Application.User.Services;
using TemplateService.Infrastructure.Persistence;

namespace TemplateService.Application.Auth.Commands;

public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, LoginUserResult?>
{
    private readonly TemplateDbContext _dbContext;
    private readonly IPasswordHelper _passwordHelper;
    private readonly ITokenService _tokenService;
    private readonly IMapper _mapper;
    private readonly IUserHelperService _userHelperService;

    public LoginUserCommandHandler
    (
        TemplateDbContext dbContext,
        IPasswordHelper passwordHelper,
        ITokenService tokenService,
        IMapper mapper,
        IUserHelperService userHelperService
    )
    {
        _dbContext = dbContext;
        _passwordHelper = passwordHelper;
        _tokenService = tokenService;
        _mapper = mapper;
        _userHelperService = userHelperService;
    }

    public async Task<LoginUserResult?> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Login == request.Login, cancellationToken);

        if (user == null || !_passwordHelper.VerifyPassword(user.PasswordHash, request.Password))
        {
            return null;
        }
        
        var userDto = _userHelperService.BuildUserDto(user, _mapper);

        var token = _tokenService.CreateToken(userDto);

        return new LoginUserResult(userDto, token);
    }
}