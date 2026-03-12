using System.Net;
using ConcurExpense.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ConcurExpense;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers the Concur Expense API client and all required services.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <param name="configure">Delegate to configure <see cref="ConcurOptions"/>.</param>
    public static IServiceCollection AddConcurExpenseClient(
        this IServiceCollection services,
        Action<ConcurOptions> configure)
    {
        services.Configure(configure);

        services.AddSingleton<IConcurTokenService, ConcurTokenService>();
        services.AddTransient<IConcurExpenseClient, ConcurExpenseClient>();

        // Use a factory so we can read options at HttpClient build time
        services.AddHttpClient(ConcurHttpClients.Auth)
            .ConfigurePrimaryHttpMessageHandler(sp =>
                BuildHandler(sp.GetRequiredService<IOptions<ConcurOptions>>().Value));

        services.AddHttpClient(ConcurHttpClients.Api)
            .ConfigurePrimaryHttpMessageHandler(sp =>
                BuildHandler(sp.GetRequiredService<IOptions<ConcurOptions>>().Value));

        return services;
    }

    private static HttpClientHandler BuildHandler(ConcurOptions options)
    {
        var handler = new HttpClientHandler();

        if (!string.IsNullOrWhiteSpace(options.ProxyUrl))
        {
            var proxy = new WebProxy(options.ProxyUrl);

            if (!string.IsNullOrWhiteSpace(options.ProxyUsername) &&
                !string.IsNullOrWhiteSpace(options.ProxyPassword))
            {
                proxy.Credentials = new NetworkCredential(
                    options.ProxyUsername,
                    options.ProxyPassword);
            }

            handler.Proxy = proxy;
            handler.UseProxy = true;
        }

        return handler;
    }
}
