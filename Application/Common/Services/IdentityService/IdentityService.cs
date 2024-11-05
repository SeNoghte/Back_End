using Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Application.Common.Services.IdentityService;

public class IdentityService : IIdentityService
{
    IConfiguration configuration { get; set; }
    public IdentityService(IConfiguration configuration)
    {
        this.configuration = configuration;
    }
    public (byte[] hash, byte[] salt) CreatePasswordHash(string password)
    {
        using var hmac = new HMACSHA512();
        var salt = hmac.Key;
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        return (hash, salt);
    }

    public bool VerifyPassword(string password, string passwordHash, string passwordSalt)
    {
        byte[] storedHash = Convert.FromBase64String(passwordHash);
        byte[] storedSalt = Convert.FromBase64String(passwordSalt);
        using var hmac = new HMACSHA512(storedSalt);
        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        return computedHash.SequenceEqual(storedHash);
    }

    public IEnumerable<Claim> GetClaimsFromToken(string accessToken)
    {
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(accessToken);
        return jwtToken.Claims;
    }

    public string GenerateJwtToken(User user)
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
