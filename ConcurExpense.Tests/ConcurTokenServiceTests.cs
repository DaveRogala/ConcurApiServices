using ConcurExpense.Services;
using ConcurExpense.Tests.Helpers;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace ConcurExpense.Tests;

public class ConcurTokenServiceTests
{
    private static (ConcurTokenService Service, FakeHttpMessageHandler Handler) Create(ConcurOptions? options = null)
    {
        var handler = new FakeHttpMessageHandler();
        var factory = new Mock<IHttpClientFactory>();
        factory.Setup(f => f.CreateClient(It.IsAny<string>()))
               .Returns(() => new HttpClient(handler));

        var opts = Options.Create(options ?? new ConcurOptions
        {
            ClientId = "client-id",
            ClientSecret = "client-secret",
            RefreshToken = "refresh-token"
        });

        var service = new ConcurTokenService(
            factory.Object,
            opts,
            NullLogger<ConcurTokenService>.Instance);

        return (service, handler);
    }

    [Fact]
    public async Task GetAccessToken_ReturnsTokenFromApi()
    {
        var (service, handler) = Create();
        handler.Enqueue(JsonPayloads.TokenResponse("my-access-token"));

        var token = await service.GetAccessTokenAsync();

        Assert.Equal("my-access-token", token);
    }

    [Fact]
    public async Task GetAccessToken_PostsToTokenEndpointWithCorrectFormFields()
    {
        var (service, handler) = Create(new ConcurOptions
        {
            ClientId = "the-client-id",
            ClientSecret = "the-client-secret",
            RefreshToken = "the-refresh-token"
        });
        handler.Enqueue(JsonPayloads.TokenResponse());

        await service.GetAccessTokenAsync();

        var request = handler.Requests[0];
        Assert.Equal(HttpMethod.Post, request.Method);

        var body = await request.Content!.ReadAsStringAsync();
        Assert.Contains("grant_type=refresh_token", body);
        Assert.Contains("client_id=the-client-id", body);
        Assert.Contains("client_secret=the-client-secret", body);
        Assert.Contains("refresh_token=the-refresh-token", body);
    }

    [Fact]
    public async Task GetAccessToken_CachesTokenOnSubsequentCalls()
    {
        var (service, handler) = Create();
        handler.Enqueue(JsonPayloads.TokenResponse("cached-token", expiresIn: 3600));

        var first = await service.GetAccessTokenAsync();
        var second = await service.GetAccessTokenAsync();

        Assert.Equal("cached-token", first);
        Assert.Equal("cached-token", second);
        // Only one HTTP call despite two GetAccessToken calls
        Assert.Single(handler.Requests);
    }

    [Fact]
    public async Task GetAccessToken_RefreshesTokenWhenExpired()
    {
        var (service, handler) = Create();
        // First token expires in 1s (after subtracting 60s buffer it's already expired)
        handler.Enqueue(JsonPayloads.TokenResponse("old-token", expiresIn: 1));
        handler.Enqueue(JsonPayloads.TokenResponse("new-token", expiresIn: 3600));

        var first = await service.GetAccessTokenAsync();
        // Force expiry by waiting (the 60s buffer means expiresIn=1 is immediately stale)
        await Task.Delay(10);
        var second = await service.GetAccessTokenAsync();

        Assert.Equal("old-token", first);
        Assert.Equal("new-token", second);
        Assert.Equal(2, handler.Requests.Count);
    }

    [Fact]
    public async Task GetAccessToken_ThrowsWhenApiReturnsError()
    {
        var (service, handler) = Create();
        handler.EnqueueError(System.Net.HttpStatusCode.Unauthorized);

        await Assert.ThrowsAsync<HttpRequestException>(() => service.GetAccessTokenAsync());
    }
}
