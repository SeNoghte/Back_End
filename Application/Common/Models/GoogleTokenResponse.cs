using System.Text.Json.Serialization;

namespace Application.Common.Models;

public class GoogleTokenResponse
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; }

    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; }

    [JsonPropertyName("token_type")]
    public string TokenType { get; set; }

    [JsonPropertyName("refresh_token")]
    public string RefreshToken { get; set; }
}