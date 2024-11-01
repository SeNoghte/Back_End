using Application.Common.Models;
using Application.Common.Services.IdentityService;
using DataAccess;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Application.Users;

public class LoginCommand:IRequest<LoginResult>
{
    public string Email { get; set; }
    public string Password { get; set; }
}

public class LoginResult:ResultModel
{
    public string? Token { get; set; }
}

public class LoginHandler : IRequestHandler<LoginCommand, LoginResult>
{
    ApplicationDBContext applicationDB {  get; set; }
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
        var user = await applicationDB.Users.SingleOrDefaultAsync(u => u.Email == request.Email);
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
