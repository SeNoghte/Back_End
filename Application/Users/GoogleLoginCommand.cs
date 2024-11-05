using Application.Common.Models;
using Application.Common.Services.IdentityService;
using DataAccess;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace Application.Users
{
    public class GoogleLoginCommand : IRequest<GoogleLoginResult>
    {
        public string AuthorizationCode { get; set; }
    }

    public class GoogleLoginResult : ResultModel
    {
        public string Token { get; set; }
    }

    public class GoogleLoginHandler : IRequestHandler<GoogleLoginCommand, GoogleLoginResult>
    {
        private readonly HttpClient _httpClient;
        private readonly IIdentityService _identityService;
        private readonly IConfiguration _configuration;
        private readonly ApplicationDBContext _dbContext;

        public GoogleLoginHandler(HttpClient httpClient, IIdentityService identityService, IConfiguration configuration, ApplicationDBContext applicationDB)
        {
            _httpClient = httpClient;
            _identityService = identityService;
            _configuration = configuration;
            _dbContext = applicationDB;
        }

        public async Task<GoogleLoginResult> Handle(GoogleLoginCommand request, CancellationToken cancellationToken)
        {
            var result = new GoogleLoginResult();

            GoogleTokenResponse GToken = await ExchangeAuthCodeForTokensAsync(request.AuthorizationCode);        
            if (GToken is null)
            {
                result.Message = "اطلاعات نامعتبر";
                return result;
            }

            var claims = _identityService.GetClaimsFromToken(GToken.IdToken);

            var email = claims.FirstOrDefault(c => c.Type == "email")?.Value;
            var name = claims.FirstOrDefault(c => c.Type == "name")?.Value;
            var (passwordHash, passwordSalt) = _identityService.CreatePasswordHash(name); //google auth => password is name

            var user = new User
            {
                Username = name,
                PasswordHash = Convert.ToBase64String(passwordHash),
                PasswordSalt = Convert.ToBase64String(passwordSalt),
                Email = email
            };

            bool userExists = await _dbContext.Users.AnyAsync(u => u.Email == email);

            if (!userExists)
            {
                _dbContext.Users.Add(user);
                await _dbContext.SaveChangesAsync();
            }

            user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);

            var jwtToken = _identityService.GenerateJwtToken(user);

            result.Token = jwtToken;
            result.Success = true;
            return result;

        }


        public async Task<GoogleTokenResponse> ExchangeAuthCodeForTokensAsync(string authorizationCode)
        {
            string? clientId = _configuration["Authentication:Google:ClientId"];
            string? clientSecret = _configuration["Authentication:Google:ClientSecret"];
            string? redirectUri = _configuration["Authentication:Google:RedirectUri"];

            var requestData = new Dictionary<string, string>
            {
                { "code", authorizationCode },
                { "client_id", clientId },
                { "client_secret", clientSecret },
                { "redirect_uri", redirectUri },
                { "grant_type", "authorization_code" }
            };

            var requestContent = new FormUrlEncodedContent(requestData);

            var response = await _httpClient.PostAsync("https://oauth2.googleapis.com/token", requestContent);
            response.EnsureSuccessStatusCode();

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var tokenResponse = JsonSerializer.Deserialize<GoogleTokenResponse>(jsonResponse);

            return tokenResponse;
        }
    }
}
