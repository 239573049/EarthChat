using System.Text.Json.Serialization;

namespace EarthChat.AuthServer.Application.Contract.Authorize.Dto;

public class OAuthUserDto
{
    [JsonPropertyName("id")] public int Id { get; set; }

    [JsonPropertyName("avatar_url")] public string? AvatarUrl { get; set; }

    [JsonPropertyName("name")] public string? Name { get; set; }

    [JsonPropertyName("email")] public string? Email { get; set; }
}