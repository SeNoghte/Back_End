using Application.Common.Models;
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
    IConfiguration configuration { get; set; }

    public LoginHandler(ApplicationDBContext applicationDB, IConfiguration configuration)
    {
        this.applicationDB = applicationDB;
        this.configuration = configuration;
    }
    public async Task<LoginResult> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var result = new LoginResult();
        var user = await applicationDB.Users.SingleOrDefaultAsync(u => u.Email == request.Email);
        if (user == null || !VerifyPassword(request.Password, user.PasswordHash, user.PasswordSalt))
        {
            result.Message = "ایمیل یا رمز عبور اشتباه است";
            return result;
        }

        var token = GenerateJwtToken(user);

        result.Success = true;
        result.Token = token;

        return result;
    }

    private bool VerifyPassword(string password, string passwordHash, string passwordSalt)
    {
        byte[] storedHash = Convert.FromBase64String(passwordHash);
        byte[] storedSalt = Convert.FromBase64String(passwordSalt);
        using var hmac = new HMACSHA512(storedSalt);
        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        return computedHash.SequenceEqual(storedHash);
    }

    private string GenerateJwtToken(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtSettings:SecretKey"]));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
        };

        var token = new JwtSecurityToken(
            configuration["JwtSettings:Issuer"],
            configuration["JwtSettings:Audience"],
            claims,
            expires: DateTime.UtcNow.AddHours(double.Parse(configuration["JwtSettings:ExpiresInHours"])),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
