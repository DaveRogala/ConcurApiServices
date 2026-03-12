using System.Net.Http.Json;
using System.Text.Json.Serialization;
using ConcurExpense.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ConcurExpense.Services;

internal class ConcurTokenService : IConcurTokenService
{
    private const string TokenEndpoint = "https://us.api.concursolutions.com/oauth2/v0/token";

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ConcurOptions _options;
    private readonly ILogger<ConcurTokenService> _logger;

    private string? _cachedToken;
    private DateTime _tokenExpiry = DateTime.MinValue;
    private readonly SemaphoreSlim _lock = new(1, 1);

    public ConcurTokenService(
        IHttpClientFactory httpClientFactory,
        IOptions<ConcurOptions> options,
        ILogger<ConcurTokenService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<string> GetAccessTokenAsync(CancellationToken cancellationToken = default)
    {
        if (_cachedToken is not null && DateTime.UtcNow < _tokenExpiry)
            return _cachedToken;

        await _lock.WaitAsync(cancellationToken);
        try
        {
            // Double-check after acquiring lock
            if (_cachedToken is not null && DateTime.UtcNow < _tokenExpiry)
                return _cachedToken;

            _logger.LogDebug("Refreshing Concur access token.");

            var client = _httpClientFactory.CreateClient(ConcurHttpClients.Auth);

            var formData = new Dictionary<string, string>
            {
                ["grant_type"] = "refresh_token",
                ["client_id"] = _options.ClientId,
                ["client_secret"] = _options.ClientSecret,
                ["refresh_token"] = _options.RefreshToken
            };

            using var response = await client.PostAsync(
                TokenEndpoint,
                new FormUrlEncodedContent(formData),
                cancellationToken);

            response.EnsureSuccessStatusCode();

            var tokenResponse = await response.Content.ReadFromJsonAsync<TokenResponse>(cancellationToken: cancellationToken)
                ?? throw new InvalidOperationException("Failed to deserialize token response.");

            _cachedToken = tokenResponse.AccessToken;
            // Expire 60 seconds before actual expiry to avoid edge cases
            _tokenExpiry = DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn - 60);

            _logger.LogDebug("Concur access token refreshed successfully. Expires in {ExpiresIn}s.", tokenResponse.ExpiresIn);

            return _cachedToken;
        }
        finally
        {
            _lock.Release();
        }
    }
}
