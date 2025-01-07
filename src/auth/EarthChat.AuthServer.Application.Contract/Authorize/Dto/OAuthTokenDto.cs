using System.Text.Json.Serialization;

namespace EarthChat.AuthServer.Application.Contract.Authorize.Dto;

public class OAuthTokenDto
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; } = null!;
}