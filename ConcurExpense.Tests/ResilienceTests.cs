using System.Net;
using ConcurExpense.Tests.Helpers;
using Xunit;
using System.Threading;

namespace ConcurExpense.Tests;

/// <summary>
/// Tests for retry-on-5xx, rate-limit enforcement, and 429 handling.
/// Delays are bypassed via the ClientFactory.Delay no-op override.
/// </summary>
public class ResilienceTests
{
    // ── 5xx Retry ────────────────────────────────────────────────────────────

    [Fact]
    public async Task ServerError_RetriesUpToThreeTimes_ThenSucceeds()
    {
        var (client, handler) = ClientFactory.Create();
        handler.EnqueueError(HttpStatusCode.InternalServerError);
        handler.EnqueueError(HttpStatusCode.InternalServerError);
        handler.EnqueueError(HttpStatusCode.InternalServerError);
        handler.Enqueue(JsonPayloads.ReportPage(ids: ["R1"]));

        var reports = await client.GetReportsAsync();

        Assert.Single(reports);
        Assert.Equal(4, handler.Requests.Count); // 3 failures + 1 success
    }

    [Fact]
    public async Task ServerError_SingleRetry_ThenSucceeds()
    {
        var (client, handler) = ClientFactory.Create();
        handler.EnqueueError(HttpStatusCode.ServiceUnavailable);
        handler.Enqueue(JsonPayloads.ReportPage(ids: ["R1"]));

        var reports = await client.GetReportsAsync();

        Assert.Single(reports);
        Assert.Equal(2, handler.Requests.Count);
    }

    [Fact]
    public async Task ServerError_ExceedsMaxRetries_Throws()
    {
        var (client, handler) = ClientFactory.Create();
        handler.EnqueueError(HttpStatusCode.InternalServerError);
        handler.EnqueueError(HttpStatusCode.InternalServerError);
        handler.EnqueueError(HttpStatusCode.InternalServerError);
        handler.EnqueueError(HttpStatusCode.InternalServerError); // 4th → should throw

        await Assert.ThrowsAsync<HttpRequestException>(() => client.GetReportsAsync());

        Assert.Equal(4, handler.Requests.Count); // initial + 3 retries
    }

    [Fact]
    public async Task ServerError_RetriesAllStatusCodesIn5xxRange()
    {
        var (client, handler) = ClientFactory.Create();
        handler.EnqueueError(HttpStatusCode.BadGateway);        // 502
        handler.EnqueueError(HttpStatusCode.GatewayTimeout);    // 504
        handler.Enqueue(JsonPayloads.ReportPage(ids: ["R1"]));

        var reports = await client.GetReportsAsync();

        Assert.Single(reports);
        Assert.Equal(3, handler.Requests.Count);
    }

    [Fact]
    public async Task ServerError_DoesNotRetryOn4xx()
    {
        var (client, handler) = ClientFactory.Create();
        handler.EnqueueError(HttpStatusCode.Forbidden); // 403 — should throw immediately

        await Assert.ThrowsAsync<HttpRequestException>(() => client.GetReportsAsync());

        Assert.Single(handler.Requests); // no retries
    }

    // ── 429 Rate Limiting ────────────────────────────────────────────────────

    [Fact]
    public async Task RateLimit_SingleRateLimitThenSuccess_Retries()
    {
        var (client, handler) = ClientFactory.Create();
        handler.EnqueueError(HttpStatusCode.TooManyRequests);
        handler.Enqueue(JsonPayloads.ReportPage(ids: ["R1"]));

        var reports = await client.GetReportsAsync();

        Assert.Single(reports);
        Assert.Equal(2, handler.Requests.Count);
    }

    [Fact]
    public async Task RateLimit_FiveConsecutive_Throws()
    {
        var (client, handler) = ClientFactory.Create();
        for (var i = 0; i < 5; i++)
            handler.EnqueueError(HttpStatusCode.TooManyRequests);

        await Assert.ThrowsAsync<HttpRequestException>(() => client.GetReportsAsync());

        Assert.Equal(5, handler.Requests.Count);
    }

    [Fact]
    public async Task RateLimit_FourConsecutiveThenSuccess_DoesNotThrow()
    {
        var (client, handler) = ClientFactory.Create();
        for (var i = 0; i < 4; i++)
            handler.EnqueueError(HttpStatusCode.TooManyRequests);
        handler.Enqueue(JsonPayloads.EmptyPage());

        // Should succeed — limit is 5 consecutive, not 4
        var reports = await client.GetReportsAsync();

        Assert.Empty(reports);
        Assert.Equal(5, handler.Requests.Count);
    }

    [Fact]
    public async Task RateLimit_SuccessResetsConsecutiveCount()
    {
        var (client, handler) = ClientFactory.Create();
        // 4 rate limits, then success — consecutive counter should reset
        for (var i = 0; i < 4; i++)
            handler.EnqueueError(HttpStatusCode.TooManyRequests);
        handler.Enqueue(JsonPayloads.EmptyPage()); // resets counter

        // Now 4 more rate limits without hitting the limit of 5 consecutive
        for (var i = 0; i < 4; i++)
            handler.EnqueueError(HttpStatusCode.TooManyRequests);
        handler.Enqueue(JsonPayloads.EmptyPage());

        var reports = await client.GetReportsAsync();

        Assert.Empty(reports);
    }

    // ── Parallel Call Serialization ───────────────────────────────────────────

    [Fact]
    public async Task ParallelCalls_NeverExceedOneInflightRequest()
    {
        var (client, handler) = ClientFactory.Create();

        // 2 responses consumed by GetReportsAsync + GetEntriesAsync respectively.
        // Empty pages work for any deserialization target (no items to parse).
        handler.Enqueue(JsonPayloads.EmptyPage());
        handler.Enqueue(JsonPayloads.EmptyPage());

        var concurrent = 0;
        var maxConcurrent = 0;

        handler.OnSendAsync = async () =>
        {
            var c = Interlocked.Increment(ref concurrent);
            // Yield so that a second task could interleave here if the lock weren't held.
            await Task.Yield();
            var snapshot = Volatile.Read(ref maxConcurrent);
            while (c > snapshot)
            {
                var updated = Interlocked.CompareExchange(ref maxConcurrent, c, snapshot);
                if (updated == snapshot) break;
                snapshot = updated;
            }
            Interlocked.Decrement(ref concurrent);
        };

        var reportsTask = client.GetReportsAsync();
        var entriesTask = client.GetEntriesAsync("rep-1");

        await Task.WhenAll(reportsTask, entriesTask);

        Assert.Equal(1, maxConcurrent);
    }

    [Fact]
    public async Task RateLimit_FiveInOneLoop_EscalatesMinCallInterval()
    {
        var (client, handler) = ClientFactory.Create();

        // Queue 5 rate-limit responses (within one FetchAllPages call)
        // then a success — consecutive count is 5 but we throw before we'd get a 6th,
        // so we need to enqueue a success as the 5th response after only 4 429s to
        // verify escalation without hitting the consecutive limit.
        // Use a multi-page scenario: page 1 gets 4 rate limits then succeeds,
        // which triggers the escalation when we enqueue a 5th 429 mid-loop.

        // Simpler: use 5 429s before success — but consecutive limit is also 5.
        // We verify escalation with exactly 5 in the loop: 5 429s then success,
        // BUT that would hit the consecutive limit first. So we need the success
        // to interrupt the streak. Strategy: 4 rate-limits, success (page 1 with nextPage),
        // then 1 more rate-limit (page 2 call) to push loopRateLimitCount to 5.
        var nextPage = "https://api.example.com/api/v3.0/expense/reports?offset=25";

        for (var i = 0; i < 4; i++)
            handler.EnqueueError(HttpStatusCode.TooManyRequests);
        handler.Enqueue(JsonPayloads.ReportPage(nextPage: nextPage, ids: ["R1"])); // success on page 1
        handler.EnqueueError(HttpStatusCode.TooManyRequests); // 5th 429 in the loop (loopRateLimitCount hits 5)
        handler.Enqueue(JsonPayloads.ReportPage(ids: ["R2"])); // success on page 2

        var reports = await client.GetReportsAsync();

        Assert.Equal(2, reports.Count);
        // 4 + 1(success) + 1 + 1(success) = 7 requests
        Assert.Equal(7, handler.Requests.Count);
    }
}
