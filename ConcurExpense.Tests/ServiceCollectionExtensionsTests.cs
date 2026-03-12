using ConcurExpense.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Xunit;

namespace ConcurExpense.Tests;

public class ServiceCollectionExtensionsTests
{
    private static ServiceProvider BuildProvider(Action<ConcurOptions>? configure = null)
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddConcurExpenseClient(configure ?? (o =>
        {
            o.ClientId = "cid";
            o.ClientSecret = "csecret";
            o.RefreshToken = "rtoken";
        }));
        return services.BuildServiceProvider();
    }

    [Fact]
    public void AddConcurExpenseClient_RegistersIConcurExpenseClient()
    {
        using var provider = BuildProvider();
        var client = provider.GetService<IConcurExpenseClient>();
        Assert.NotNull(client);
    }

    [Fact]
    public void AddConcurExpenseClient_RegistersIConcurTokenService()
    {
        using var provider = BuildProvider();
        var tokenService = provider.GetService<IConcurTokenService>();
        Assert.NotNull(tokenService);
    }

    [Fact]
    public void AddConcurExpenseClient_ConfiguresOptions()
    {
        using var provider = BuildProvider(o =>
        {
            o.BaseUrl = "https://eu.api.concursolutions.com";
            o.ClientId = "my-client-id";
            o.ClientSecret = "my-secret";
            o.RefreshToken = "my-refresh";
        });

        var options = provider.GetRequiredService<IOptions<ConcurOptions>>().Value;

        Assert.Equal("https://eu.api.concursolutions.com", options.BaseUrl);
        Assert.Equal("my-client-id", options.ClientId);
        Assert.Equal("my-secret", options.ClientSecret);
        Assert.Equal("my-refresh", options.RefreshToken);
    }

    [Fact]
    public void AddConcurExpenseClient_DefaultBaseUrlIsSet()
    {
        using var provider = BuildProvider();
        var options = provider.GetRequiredService<IOptions<ConcurOptions>>().Value;
        Assert.Equal("https://us.api.concursolutions.com", options.BaseUrl);
    }

    [Fact]
    public void AddConcurExpenseClient_ProxyOptionsAreNull_WhenNotConfigured()
    {
        using var provider = BuildProvider();
        var options = provider.GetRequiredService<IOptions<ConcurOptions>>().Value;
        Assert.Null(options.ProxyUrl);
        Assert.Null(options.ProxyUsername);
        Assert.Null(options.ProxyPassword);
    }

    [Fact]
    public void AddConcurExpenseClient_ProxyOptionsArePreserved()
    {
        using var provider = BuildProvider(o =>
        {
            o.ProxyUrl = "http://proxy.example.com:8080";
            o.ProxyUsername = "proxyuser";
            o.ProxyPassword = "proxypass";
        });

        var options = provider.GetRequiredService<IOptions<ConcurOptions>>().Value;

        Assert.Equal("http://proxy.example.com:8080", options.ProxyUrl);
        Assert.Equal("proxyuser", options.ProxyUsername);
        Assert.Equal("proxypass", options.ProxyPassword);
    }

    [Fact]
    public void AddConcurExpenseClient_IConcurExpenseClientIsTransient()
    {
        using var provider = BuildProvider();
        var a = provider.GetRequiredService<IConcurExpenseClient>();
        var b = provider.GetRequiredService<IConcurExpenseClient>();
        Assert.NotSame(a, b);
    }

    [Fact]
    public void AddConcurExpenseClient_IConcurTokenServiceIsSingleton()
    {
        using var provider = BuildProvider();
        var a = provider.GetRequiredService<IConcurTokenService>();
        var b = provider.GetRequiredService<IConcurTokenService>();
        Assert.Same(a, b);
    }
}
