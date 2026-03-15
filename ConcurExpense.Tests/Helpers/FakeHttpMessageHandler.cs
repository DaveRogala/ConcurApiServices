using System.Net;

namespace ConcurExpense.Tests.Helpers;

/// <summary>
/// An HttpMessageHandler that returns pre-queued responses in order,
/// and records every request it receives for later assertion.
/// </summary>
internal class FakeHttpMessageHandler : HttpMessageHandler
{
    private readonly Queue<HttpResponseMessage> _responses = new();
    private readonly List<HttpRequestMessage> _requests = [];

    public IReadOnlyList<HttpRequestMessage> Requests => _requests;

    public void Enqueue(string json, HttpStatusCode status = HttpStatusCode.OK)
    {
        _responses.Enqueue(new HttpResponseMessage(status)
        {
            Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json")
        });
    }

    public void EnqueueError(HttpStatusCode status) =>
        _responses.Enqueue(new HttpResponseMessage(status));

    /// <summary>
    /// Optional hook called at the start of each request, before the response is returned.
    /// Use this to simulate latency or track concurrency in tests.
    /// </summary>
    public Func<Task>? OnSendAsync { get; set; }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        _requests.Add(request);

        if (OnSendAsync is not null)
            await OnSendAsync();

        if (_responses.TryDequeue(out var response))
            return response;

        throw new InvalidOperationException(
            $"FakeHttpMessageHandler has no more queued responses (request #{_requests.Count}: {request.RequestUri}).");
    }
}
