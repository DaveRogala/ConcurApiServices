using System.Text.Json.Serialization;

namespace ConcurExpense.Models;

internal class ApiResponse<T>
{
    public List<T> Items { get; set; } = [];
    public string? NextPage { get; set; }
    public int? TotalCount { get; set; }
}

internal class TokenResponse
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; } = string.Empty;

    [JsonPropertyName("token_type")]
    public string TokenType { get; set; } = string.Empty;

    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; }

    [JsonPropertyName("refresh_token")]
    public string? RefreshToken { get; set; }
}
