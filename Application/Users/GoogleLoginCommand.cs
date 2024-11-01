using Application.Common.Models;
using Application.Common.Services.IdentityService;
using DataAccess;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Security.Claims;
using System.Text.Json;

namespace Application.Users
{
    public class GoogleLoginCommand : IRequest<GoogleLoginResult>
    {
        public string Code { get; set; } 
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

            //var accessToken = await ExchangeAuthCodeForAccessTokenAsync(request.Code);
            var accessToken = "eyJhbGciOiJSUzI1NiIsImtpZCI6ImM4OGQ4MDlmNGRiOTQzZGY1M2RhN2FjY2ZkNDc3NjRkMDViYTM5MWYiLCJ0eXAiOiJKV1QifQ.eyJpc3MiOiJodHRwczovL2FjY291bnRzLmdvb2dsZS5jb20iLCJhenAiOiI0MDc0MDg3MTgxOTIuYXBwcy5nb29nbGV1c2VyY29udGVudC5jb20iLCJhdWQiOiI0MDc0MDg3MTgxOTIuYXBwcy5nb29nbGV1c2VyY29udGVudC5jb20iLCJzdWIiOiIxMDkzNzQ5OTcxNzYzODQ5NTQ4MDEiLCJlbWFpbCI6Im1hZGFuaXBvdXI2NkBnbWFpbC5jb20iLCJlbWFpbF92ZXJpZmllZCI6dHJ1ZSwiYXRfaGFzaCI6IkJNX0pvZ2hvZl9vSWdBREN1MW5sYVEiLCJpYXQiOjE3MzA0NzcyODAsImV4cCI6MTczMDQ4MDg4MH0.zryQNPOSxx1nO_tp34udAD5G1TSxD5a5lsGXLA8nl1_5fWYL-J5AH5_KngKcSV076QA8-cRm2owIysxTZpaMQHLXqg_vtKelxcyUM04J7x2gsv1DESHVn3t-IdnCjZUgnyHyfIqUn1KJxuzI_k3df_Lr9Y0h6NS-TiFPccDxwAX4kqEEhhuDVQkKfuSfs3j17R2OxMZXUiu24l-x_ZYbZyi5CuTA1mJLERzPCvE01m6ib-0VLlCCERX2MFYkNh_DxuGAW10LfFIfXnetwi59JbhLD2QjmNjqwDO4sqx3C9gPBKh5fubiPSCCFnmcjfRK096M9cQ6bz3Adi_YukUsOQ";

            if (string.IsNullOrEmpty(accessToken))
            {
                result.Message = "اطلاعات نامعتبر";
                return result;
            }

            var claims = _identityService.GetClaimsFromToken(accessToken);

            var email = claims.FirstOrDefault(c => c.Type == "email")?.Value;
            var name = claims.FirstOrDefault(c => c.Type == "name")?.Value;
            name = "mahdi";
            var (passwordHash, passwordSalt) = _identityService.CreatePasswordHash(name); //google auth => password is name

            var user = new User
            {
                Username = name,
                PasswordHash = Convert.ToBase64String(passwordHash),
                PasswordSalt = Convert.ToBase64String(passwordSalt),
                Email = email
            };

            var jwtToken = _identityService.GenerateJwtToken(user);

            bool userExists = await _dbContext.Users.AnyAsync(u => u.Email == email);

            if (!userExists)
            {
                _dbContext.Users.Add(user);
                await _dbContext.SaveChangesAsync();      
            }

            result.Token = jwtToken;
            result.Success = true;
            return result;

        }


        public async Task<string> ExchangeAuthCodeForAccessTokenAsync(string authorizationCode)
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

            return tokenResponse?.AccessToken;
        }
    }
}
