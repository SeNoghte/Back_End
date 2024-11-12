using Domain.Entities;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Services.IdentityService;

public interface IIdentityService
{
    public (byte[] hash, byte[] salt) CreatePasswordHash(string password);
    public bool VerifyPassword(string password, string passwordHash, string passwordSalt);
    public IEnumerable<Claim> GetClaimsFromToken(string accessToken);
    public string GenerateJwtToken(User user);
    public Guid? GetCurrentUserId();
}
