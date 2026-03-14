using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Web;
using ConcurExpense.Models;
using ConcurExpense.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ConcurExpense;

internal class ConcurExpenseClient : IConcurExpenseClient
{
    private const string ReportsPath = "/api/v3.0/expense/reports";
    private const string EntriesPath = "/api/v3.0/expense/entries";
    private const string ItemizationsPath = "/api/v3.0/expense/itemizations";
    private const string AllocationsPath = "/api/v3.0/expense/allocations";

    private const int MaxServerErrorRetries = 3;
    private const int MaxRateLimitPerLoop = 5;
    private const int MaxConsecutiveRateLimits = 5;

    private static readonly TimeSpan DefaultMinCallInterval = TimeSpan.FromMilliseconds(100);
    private static readonly TimeSpan ThrottledMinCallInterval = TimeSpan.FromSeconds(1);
    private static readonly TimeSpan ServerErrorRetryDelay = TimeSpan.FromSeconds(10);
    private static readonly TimeSpan RateLimitRetryDelay = TimeSpan.FromSeconds(10);

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConcurTokenService _tokenService;
    private readonly ILogger<ConcurExpenseClient> _logger;
    private readonly string _baseUrl;

    // Rate-limit state
    private TimeSpan _minCallInterval = DefaultMinCallInterval;
    private DateTime _lastCallTime = DateTime.MinValue;
    private readonly SemaphoreSlim _callLock = new(1, 1);
    private int _consecutiveRateLimitCount = 0;

    /// <summary>
    /// Overridable delay function — replace with a no-op in tests to avoid real waits.
    /// </summary>
    internal Func<TimeSpan, CancellationToken, Task> Delay { get; set; } = Task.Delay;

    public ConcurExpenseClient(
        IHttpClientFactory httpClientFactory,
        IConcurTokenService tokenService,
        IOptions<ConcurOptions> options,
        ILogger<ConcurExpenseClient> logger)
    {
        _httpClientFactory = httpClientFactory;
        _tokenService = tokenService;
        _logger = logger;
        _baseUrl = options.Value.BaseUrl.TrimEnd('/');
    }

    public Task<List<ReportDto>> GetReportsAsync(
        int? limit = null,
        DateTime? modifiedDateBefore = null,
        DateTime? modifiedDateAfter = null,
        string? user = null,
        string? approvalStatusCode = null,
        CancellationToken cancellationToken = default)
        => GetReportsAsync(new ReportQueryOptions
        {
            Limit = limit,
            ModifiedDateBefore = modifiedDateBefore,
            ModifiedDateAfter = modifiedDateAfter,
            User = user,
            ApprovalStatusCode = approvalStatusCode,
        }, cancellationToken);

    public async Task<List<ReportDto>> GetReportsAsync(
        ReportQueryOptions options,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(options);

        var query = BuildQuery(q =>
        {
            q["user"] = options.User ?? "ALL";
            if (options.Limit.HasValue)                       q["limit"]                        = options.Limit.Value.ToString();
            if (options.ApprovalStatusCode is { Length: > 0 }) q["approvalStatusCode"]           = options.ApprovalStatusCode;
            if (options.PaymentStatusCode is { Length: > 0 })  q["paymentStatusCode"]            = options.PaymentStatusCode;
            if (options.CurrencyCode is { Length: > 0 })       q["currencyCode"]                 = options.CurrencyCode;
            if (options.PaymentType is { Length: > 0 })        q["paymentType"]                  = options.PaymentType;
            if (options.ReimbursementMethod is { Length: > 0 }) q["reimbursementMethod"]         = options.ReimbursementMethod;
            if (options.ApproverLoginID is { Length: > 0 })    q["approverLoginID"]              = options.ApproverLoginID;
            if (options.ExpenseTypeCode is { Length: > 0 })    q["expenseTypeCode"]              = options.ExpenseTypeCode;
            if (options.AttendeeTypeCode is { Length: > 0 })   q["attendeeTypeCode"]             = options.AttendeeTypeCode;
            if (options.CountryCode is { Length: > 0 })        q["countryCode"]                  = options.CountryCode;
            if (options.BatchID is { Length: > 0 })            q["batchID"]                      = options.BatchID;
            if (options.VendorName is { Length: > 0 })         q["vendorName"]                   = options.VendorName;
            if (options.ExpenseGroupConfigID is { Length: > 0 }) q["expenseGroupConfigID"]       = options.ExpenseGroupConfigID;
            if (options.HasVAT.HasValue)                       q["hasVAT"]                       = options.HasVAT.Value.ToString().ToLowerInvariant();
            if (options.HasImages.HasValue)                    q["hasImages"]                    = options.HasImages.Value.ToString().ToLowerInvariant();
            if (options.HasAttendees.HasValue)                 q["hasAttendees"]                 = options.HasAttendees.Value.ToString().ToLowerInvariant();
            if (options.HasBillableExpenses.HasValue)          q["hasBillableExpenses"]          = options.HasBillableExpenses.Value.ToString().ToLowerInvariant();
            if (options.IsTestUser.HasValue)                   q["isTestUser"]                   = options.IsTestUser.Value.ToString().ToLowerInvariant();
            if (options.EntryTransactionDateBefore.HasValue)   q["entryTransactionDateBefore"]   = options.EntryTransactionDateBefore.Value.ToString("yyyy-MM-ddTHH:mm:ss");
            if (options.EntryTransactionDateAfter.HasValue)    q["entryTransactionDateAfter"]    = options.EntryTransactionDateAfter.Value.ToString("yyyy-MM-ddTHH:mm:ss");
            if (options.CreateDateBefore.HasValue)             q["createDateBefore"]             = options.CreateDateBefore.Value.ToString("yyyy-MM-ddTHH:mm:ss");
            if (options.CreateDateAfter.HasValue)              q["createDateAfter"]              = options.CreateDateAfter.Value.ToString("yyyy-MM-ddTHH:mm:ss");
            if (options.UserDefinedDateBefore.HasValue)        q["userDefinedDateBefore"]        = options.UserDefinedDateBefore.Value.ToString("yyyy-MM-ddTHH:mm:ss");
            if (options.UserDefinedDateAfter.HasValue)         q["userDefinedDateAfter"]         = options.UserDefinedDateAfter.Value.ToString("yyyy-MM-ddTHH:mm:ss");
            if (options.SubmitDateBefore.HasValue)             q["submitDateBefore"]             = options.SubmitDateBefore.Value.ToString("yyyy-MM-ddTHH:mm:ss");
            if (options.SubmitDateAfter.HasValue)              q["submitDateAfter"]              = options.SubmitDateAfter.Value.ToString("yyyy-MM-ddTHH:mm:ss");
            if (options.ProcessingPaymentDateBefore.HasValue)  q["processingPaymentDateBefore"]  = options.ProcessingPaymentDateBefore.Value.ToString("yyyy-MM-ddTHH:mm:ss");
            if (options.ProcessingPaymentDateAfter.HasValue)   q["processingPaymentDateAfter"]   = options.ProcessingPaymentDateAfter.Value.ToString("yyyy-MM-ddTHH:mm:ss");
            if (options.PaidDateBefore.HasValue)               q["paidDateBefore"]               = options.PaidDateBefore.Value.ToString("yyyy-MM-ddTHH:mm:ss");
            if (options.PaidDateAfter.HasValue)                q["paidDateAfter"]                = options.PaidDateAfter.Value.ToString("yyyy-MM-ddTHH:mm:ss");
            if (options.ModifiedDateBefore.HasValue)           q["modifiedDateBefore"]           = options.ModifiedDateBefore.Value.ToString("yyyy-MM-ddTHH:mm:ss");
            if (options.ModifiedDateAfter.HasValue)            q["modifiedDateAfter"]            = options.ModifiedDateAfter.Value.ToString("yyyy-MM-ddTHH:mm:ss");
        });

        var url = $"{_baseUrl}{ReportsPath}?{query}";

        _logger.LogInformation("Fetching Concur reports with user={User}, approvalStatusCode={ApprovalStatusCode}.",
            options.User ?? "ALL", options.ApprovalStatusCode);

        return await FetchAllPagesAsync<ReportDto>(url, cancellationToken);
    }

    public async Task<List<EntryDto>> GetEntriesAsync(
        string reportId,
        int? limit = null,
        string? user = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(reportId);

        var query = BuildQuery(q =>
        {
            q["user"] = user ?? "ALL";
            q["reportID"] = reportId;
            if (limit.HasValue) q["limit"] = limit.Value.ToString();
        });

        var url = $"{_baseUrl}{EntriesPath}?{query}";

        _logger.LogInformation("Fetching Concur entries for report {ReportId}.", reportId);

        return await FetchAllPagesAsync<EntryDto>(url, cancellationToken);
    }

    public async Task<List<ItemizationDto>> GetItemizationsAsync(
        string reportId,
        string? entryId = null,
        int? limit = null,
        string? user = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(reportId);

        var query = BuildQuery(q =>
        {
            q["user"] = user ?? "ALL";
            q["reportID"] = reportId;
            if (!string.IsNullOrWhiteSpace(entryId)) q["entryID"] = entryId;
            if (limit.HasValue) q["limit"] = limit.Value.ToString();
        });

        if (!string.IsNullOrWhiteSpace(entryId))
            _logger.LogInformation("Fetching Concur itemizations for report {ReportId}, entry {EntryId}.", reportId, entryId);
        else
            _logger.LogInformation("Fetching Concur itemizations for report {ReportId}.", reportId);

        var url = $"{_baseUrl}{ItemizationsPath}?{query}";
        return await FetchAllPagesAsync<ItemizationDto>(url, cancellationToken);
    }

    public async Task<List<AllocationDto>> GetAllocationsAsync(
        string reportId,
        string? entryId = null,
        string? itemizationId = null,
        int? limit = null,
        string? user = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(reportId);

        var query = BuildQuery(q =>
        {
            q["user"] = user ?? "ALL";
            q["reportID"] = reportId;
            if (!string.IsNullOrWhiteSpace(entryId)) q["entryID"] = entryId;
            if (!string.IsNullOrWhiteSpace(itemizationId)) q["itemizationID"] = itemizationId;
            if (limit.HasValue) q["limit"] = limit.Value.ToString();
        });

        var url = $"{_baseUrl}{AllocationsPath}?{query}";

        _logger.LogInformation("Fetching Concur allocations for report {ReportId}.", reportId);

        return await FetchAllPagesAsync<AllocationDto>(url, cancellationToken);
    }

    private async Task<List<T>> FetchAllPagesAsync<T>(string initialUrl, CancellationToken cancellationToken)
    {
        var allItems = new List<T>();
        string? nextUrl = initialUrl;
        var pageNumber = 1;
        var loopState = new LoopState();

        while (nextUrl is not null)
        {
            _logger.LogDebug("Fetching page {Page}: {Url}", pageNumber, nextUrl);

            var page = await GetPageAsync<T>(nextUrl, loopState, cancellationToken);

            allItems.AddRange(page.Items);
            nextUrl = page.NextPage;
            pageNumber++;
        }

        return allItems;
    }

    private async Task<ApiResponse<T>> GetPageAsync<T>(
        string url,
        LoopState loopState,
        CancellationToken cancellationToken)
    {
        var serverErrorAttempts = 0;

        while (true)
        {
            await EnforceRateLimitAsync(cancellationToken);

            var token = await _tokenService.GetAccessTokenAsync(cancellationToken);
            var client = _httpClientFactory.CreateClient(ConcurHttpClients.Api);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            using var response = await client.GetAsync(url, cancellationToken);

            // ── 429 Too Many Requests ────────────────────────────────────────
            if (response.StatusCode == HttpStatusCode.TooManyRequests)
            {
                loopState.RateLimitCount++;
                _consecutiveRateLimitCount++;

                _logger.LogWarning(
                    "Rate limited (429) — consecutive: {Consecutive}, this-loop: {LoopCount}.",
                    _consecutiveRateLimitCount, loopState.RateLimitCount);

                if (_consecutiveRateLimitCount >= MaxConsecutiveRateLimits)
                    throw new HttpRequestException(
                        $"Aborting: received {MaxConsecutiveRateLimits} consecutive 429 responses from the Concur API.");

                if (loopState.RateLimitCount >= MaxRateLimitPerLoop && _minCallInterval < ThrottledMinCallInterval)
                {
                    _logger.LogWarning(
                        "Received {Count} rate-limit responses in this batch; increasing minimum call interval to {Interval}ms.",
                        loopState.RateLimitCount, ThrottledMinCallInterval.TotalMilliseconds);
                    _minCallInterval = ThrottledMinCallInterval;
                }

                await Delay(RateLimitRetryDelay, cancellationToken);
                continue;
            }

            // ── 5xx Server Error ─────────────────────────────────────────────
            if ((int)response.StatusCode >= 500)
            {
                serverErrorAttempts++;

                if (serverErrorAttempts > MaxServerErrorRetries)
                {
                    _logger.LogError(
                        "Server error {StatusCode} persists after {Retries} retries; giving up on {Url}.",
                        response.StatusCode, MaxServerErrorRetries, url);
                    response.EnsureSuccessStatusCode(); // throws HttpRequestException
                }

                _logger.LogWarning(
                    "Server error {StatusCode} (attempt {Attempt}/{Max}), retrying in {Delay}s.",
                    response.StatusCode, serverErrorAttempts, MaxServerErrorRetries,
                    ServerErrorRetryDelay.TotalSeconds);

                await Delay(ServerErrorRetryDelay, cancellationToken);
                continue;
            }

            // ── Success ──────────────────────────────────────────────────────
            response.EnsureSuccessStatusCode();
            _consecutiveRateLimitCount = 0;

            return await response.Content.ReadFromJsonAsync<ApiResponse<T>>(cancellationToken: cancellationToken)
                ?? throw new InvalidOperationException($"Failed to deserialize API response from {url}.");
        }
    }

    private sealed class LoopState
    {
        public int RateLimitCount { get; set; }
    }

    private async Task EnforceRateLimitAsync(CancellationToken cancellationToken)
    {
        await _callLock.WaitAsync(cancellationToken);
        try
        {
            var elapsed = DateTime.UtcNow - _lastCallTime;
            var wait = _minCallInterval - elapsed;
            if (wait > TimeSpan.Zero)
                await Delay(wait, cancellationToken);
            _lastCallTime = DateTime.UtcNow;
        }
        finally
        {
            _callLock.Release();
        }
    }

    private static string BuildQuery(Action<Dictionary<string, string>> configure)
    {
        var parameters = new Dictionary<string, string>();
        configure(parameters);

        var queryString = HttpUtility.ParseQueryString(string.Empty);
        foreach (var (key, value) in parameters)
            queryString[key] = value;

        return queryString.ToString() ?? string.Empty;
    }
}
