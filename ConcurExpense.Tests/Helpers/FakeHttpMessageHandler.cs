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

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        _requests.Add(request);

        if (_responses.TryDequeue(out var response))
            return Task.FromResult(response);

        throw new InvalidOperationException(
            $"FakeHttpMessageHandler has no more queued responses (request #{_requests.Count}: {request.RequestUri}).");
    }
}
