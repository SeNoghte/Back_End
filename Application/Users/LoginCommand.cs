using Application.Common.Models;
using Application.Common.Services.IdentityService;
using DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Users;

public class LoginCommand : IRequest<LoginResult>
{
    public string EmailOrUsername { get; set; }
    public string Password { get; set; }
}

public class LoginResult : ResultModel
{
    public string? Token { get; set; }
}

public class LoginHandler : IRequestHandler<LoginCommand, LoginResult>
{
    ApplicationDBContext applicationDB { get; set; }
    IIdentityService identityService { get; set; }

    public LoginHandler(ApplicationDBContext applicationDB,
        IIdentityService identityService)
    {
        this.applicationDB = applicationDB;
        this.identityService = identityService;
    }
    public async Task<LoginResult> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var result = new LoginResult();

        var user = await applicationDB.Users.SingleOrDefaultAsync(u => u.Email == request.EmailOrUsername 
                                                || u.Username == request.EmailOrUsername);

        if (user == null || !identityService.VerifyPassword(request.Password, user.PasswordHash, user.PasswordSalt))
        {
            result.Message = "ایمیل یا رمز عبور اشتباه است";
            return result;
        }

        var token = identityService.GenerateJwtToken(user);

        result.Success = true;
        result.Token = token;

        return result;
    }
}
