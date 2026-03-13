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
    private const string ReportsPath = "/expense/expensereport/v3.0/reports";
    private const string EntriesPath = "/expense/expensereport/v3.0/reports/{0}/entries";
    private const string ItemizationsPath = "/expense/expensereport/v3.0/reports/{0}/entries/{1}/itemizations";
    private const string ItemizationsReportPath = "/expense/expensereport/v3.0/reports/{0}/entries/itemizations";
    private const string AllocationsPath = "/expense/expensereport/v3.0/allocations";

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConcurTokenService _tokenService;
    private readonly ILogger<ConcurExpenseClient> _logger;
    private readonly string _baseUrl;

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

    public async Task<List<ReportDto>> GetReportsAsync(
        int? limit = null,
        DateTime? modifiedDateBefore = null,
        DateTime? modifiedDateAfter = null,
        string? user = null,
        string? approvalStatusCode = null,
        CancellationToken cancellationToken = default)
    {
        var query = BuildQuery(q =>
        {
            q["user"] = user ?? "ALL";
            if (limit.HasValue) q["limit"] = limit.Value.ToString();
            if (modifiedDateBefore.HasValue) q["modifiedDateBefore"] = modifiedDateBefore.Value.ToString("yyyy-MM-ddTHH:mm:ss");
            if (modifiedDateAfter.HasValue) q["modifiedDateAfter"] = modifiedDateAfter.Value.ToString("yyyy-MM-ddTHH:mm:ss");
            if (!string.IsNullOrWhiteSpace(approvalStatusCode)) q["approvalStatusCode"] = approvalStatusCode;
        });

        var url = $"{_baseUrl}{ReportsPath}?{query}";

        _logger.LogInformation("Fetching Concur reports with user={User}, approvalStatusCode={ApprovalStatusCode}.",
            user ?? "ALL", approvalStatusCode);

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
            if (limit.HasValue) q["limit"] = limit.Value.ToString();
        });

        var path = string.Format(EntriesPath, Uri.EscapeDataString(reportId));
        var url = $"{_baseUrl}{path}?{query}";

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
            if (limit.HasValue) q["limit"] = limit.Value.ToString();
        });

        string path;
        if (!string.IsNullOrWhiteSpace(entryId))
        {
            path = string.Format(ItemizationsPath, Uri.EscapeDataString(reportId), Uri.EscapeDataString(entryId));
            _logger.LogInformation("Fetching Concur itemizations for report {ReportId}, entry {EntryId}.", reportId, entryId);
        }
        else
        {
            path = string.Format(ItemizationsReportPath, Uri.EscapeDataString(reportId));
            _logger.LogInformation("Fetching Concur itemizations for report {ReportId}.", reportId);
        }

        var url = $"{_baseUrl}{path}?{query}";
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

        while (nextUrl is not null)
        {
            _logger.LogDebug("Fetching page {Page}: {Url}", pageNumber, nextUrl);

            var token = await _tokenService.GetAccessTokenAsync(cancellationToken);
            var client = _httpClientFactory.CreateClient(ConcurHttpClients.Api);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            using var response = await client.GetAsync(nextUrl, cancellationToken);
            response.EnsureSuccessStatusCode();

            var page = await response.Content.ReadFromJsonAsync<ApiResponse<T>>(cancellationToken: cancellationToken)
                ?? throw new InvalidOperationException($"Failed to deserialize API response from {nextUrl}.");

            allItems.AddRange(page.Items);
            nextUrl = page.NextPage;
            pageNumber++;
        }

        return allItems;
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
