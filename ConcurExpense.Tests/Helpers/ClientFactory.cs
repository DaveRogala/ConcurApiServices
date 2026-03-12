using ConcurExpense.Services;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;

namespace ConcurExpense.Tests.Helpers;

/// <summary>
/// Builds a ConcurExpenseClient wired to a FakeHttpMessageHandler for testing.
/// </summary>
internal static class ClientFactory
{
    public static (ConcurExpenseClient Client, FakeHttpMessageHandler Handler) Create(
        string baseUrl = "https://api.example.com",
        string fakeToken = "test-token")
    {
        var handler = new FakeHttpMessageHandler();

        var tokenService = new Mock<IConcurTokenService>();
        tokenService.Setup(t => t.GetAccessTokenAsync(It.IsAny<CancellationToken>()))
                    .ReturnsAsync(fakeToken);

        var factory = new Mock<IHttpClientFactory>();
        factory.Setup(f => f.CreateClient(It.IsAny<string>()))
               .Returns(() => new HttpClient(handler));

        var options = Options.Create(new ConcurOptions { BaseUrl = baseUrl });

        var client = new ConcurExpenseClient(
            factory.Object,
            tokenService.Object,
            options,
            NullLogger<ConcurExpenseClient>.Instance);

        return (client, handler);
    }
}
