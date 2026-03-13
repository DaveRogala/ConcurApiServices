using System.Web;
using ConcurExpense.Tests.Helpers;
using Xunit;

namespace ConcurExpense.Tests;

public class ConcurExpenseClientTests
{
    // ── GetReportsAsync ──────────────────────────────────────────────────────

    [Fact]
    public async Task GetReports_ReturnsMappedItems()
    {
        var (client, handler) = ClientFactory.Create();
        handler.Enqueue(JsonPayloads.ReportPage(ids: ["R1", "R2"]));

        var reports = await client.GetReportsAsync();

        Assert.Equal(2, reports.Count);
        Assert.Equal("R1", reports[0].ID);
        Assert.Equal("Report R1", reports[0].Name);
        Assert.Equal(100m, reports[0].Total);
        Assert.Equal("USD", reports[0].CurrencyCode);
        Assert.Equal("A_APPR", reports[0].ApprovalStatusCode);
    }

    [Fact]
    public async Task GetReports_MapsCustomFieldObjects()
    {
        var (client, handler) = ClientFactory.Create();
        handler.Enqueue(JsonPayloads.ReportPage(ids: ["R1"]));

        var reports = await client.GetReportsAsync();

        Assert.NotNull(reports[0].OrgUnit1);
        Assert.Equal("OU1", reports[0].OrgUnit1!.Code);
        Assert.Equal("Org Unit 1", reports[0].OrgUnit1!.Value);
        Assert.Equal("List", reports[0].OrgUnit1!.Type);
        Assert.Equal("abc", reports[0].OrgUnit1!.ListItemID);
    }

    [Fact]
    public async Task GetReports_DefaultsUserToAll()
    {
        var (client, handler) = ClientFactory.Create();
        handler.Enqueue(JsonPayloads.EmptyPage());

        await client.GetReportsAsync(user: null);

        var query = HttpUtility.ParseQueryString(handler.Requests[0].RequestUri!.Query);
        Assert.Equal("ALL", query["user"]);
    }

    [Fact]
    public async Task GetReports_UsesProvidedUser()
    {
        var (client, handler) = ClientFactory.Create();
        handler.Enqueue(JsonPayloads.EmptyPage());

        await client.GetReportsAsync(user: "someone@example.com");

        var query = HttpUtility.ParseQueryString(handler.Requests[0].RequestUri!.Query);
        Assert.Equal("someone@example.com", query["user"]);
    }

    [Fact]
    public async Task GetReports_AppliesOptionalFilters()
    {
        var (client, handler) = ClientFactory.Create();
        handler.Enqueue(JsonPayloads.EmptyPage());

        var after = new DateTime(2024, 1, 1, 0, 0, 0);
        var before = new DateTime(2024, 12, 31, 0, 0, 0);

        await client.GetReportsAsync(
            limit: 50,
            modifiedDateAfter: after,
            modifiedDateBefore: before,
            approvalStatusCode: "A_APPR");

        var query = HttpUtility.ParseQueryString(handler.Requests[0].RequestUri!.Query);
        Assert.Equal("50", query["limit"]);
        Assert.Equal("2024-01-01T00:00:00", query["modifiedDateAfter"]);
        Assert.Equal("2024-12-31T00:00:00", query["modifiedDateBefore"]);
        Assert.Equal("A_APPR", query["approvalStatusCode"]);
    }

    [Fact]
    public async Task GetReports_OmitsOptionalFiltersWhenNotProvided()
    {
        var (client, handler) = ClientFactory.Create();
        handler.Enqueue(JsonPayloads.EmptyPage());

        await client.GetReportsAsync();

        var query = HttpUtility.ParseQueryString(handler.Requests[0].RequestUri!.Query);
        Assert.Null(query["limit"]);
        Assert.Null(query["modifiedDateAfter"]);
        Assert.Null(query["modifiedDateBefore"]);
        Assert.Null(query["approvalStatusCode"]);
    }

    [Fact]
    public async Task GetReports_PaginatesThroughAllPages()
    {
        var (client, handler) = ClientFactory.Create(baseUrl: "https://api.example.com");
        handler.Enqueue(JsonPayloads.ReportPage(
            nextPage: "https://api.example.com/api/v3.0/expense/reports?offset=25&user=ALL",
            ids: ["R1"]));
        handler.Enqueue(JsonPayloads.ReportPage(ids: ["R2"]));

        var reports = await client.GetReportsAsync();

        Assert.Equal(2, reports.Count);
        Assert.Equal("R1", reports[0].ID);
        Assert.Equal("R2", reports[1].ID);
        Assert.Equal(2, handler.Requests.Count);
    }

    [Fact]
    public async Task GetReports_SecondPageUsesNextPageUrl()
    {
        var (client, handler) = ClientFactory.Create(baseUrl: "https://api.example.com");
        var nextPageUrl = "https://api.example.com/api/v3.0/expense/reports?offset=25&user=ALL";
        handler.Enqueue(JsonPayloads.ReportPage(nextPage: nextPageUrl, ids: ["R1"]));
        handler.Enqueue(JsonPayloads.ReportPage(ids: ["R2"]));

        await client.GetReportsAsync();

        Assert.Equal(new Uri(nextPageUrl), handler.Requests[1].RequestUri);
    }

    [Fact]
    public async Task GetReports_ReturnsEmptyListWhenNoItems()
    {
        var (client, handler) = ClientFactory.Create();
        handler.Enqueue(JsonPayloads.EmptyPage());

        var reports = await client.GetReportsAsync();

        Assert.Empty(reports);
    }

    [Fact]
    public async Task GetReports_UsesBaseUrlFromOptions()
    {
        var (client, handler) = ClientFactory.Create(baseUrl: "https://custom.host.com");
        handler.Enqueue(JsonPayloads.EmptyPage());

        await client.GetReportsAsync();

        Assert.StartsWith("https://custom.host.com/", handler.Requests[0].RequestUri!.ToString());
    }

    [Fact]
    public async Task GetReports_SetsBearerTokenOnRequest()
    {
        var (client, handler) = ClientFactory.Create(fakeToken: "my-secret-token");
        handler.Enqueue(JsonPayloads.EmptyPage());

        await client.GetReportsAsync();

        Assert.Equal("Bearer", handler.Requests[0].Headers.Authorization?.Scheme);
        Assert.Equal("my-secret-token", handler.Requests[0].Headers.Authorization?.Parameter);
    }

    // ── GetEntriesAsync ──────────────────────────────────────────────────────

    [Fact]
    public async Task GetEntries_ReturnsMappedItems()
    {
        var (client, handler) = ClientFactory.Create();
        handler.Enqueue(JsonPayloads.EntryPage("RPT1", ids: ["E1", "E2"]));

        var entries = await client.GetEntriesAsync("RPT1");

        Assert.Equal(2, entries.Count);
        Assert.Equal("E1", entries[0].ID);
        Assert.Equal("RPT1", entries[0].ReportID);
        Assert.Equal("MEALS", entries[0].ExpenseTypeCode);
        Assert.Equal(50m, entries[0].TransactionAmount);
    }

    [Fact]
    public async Task GetEntries_MapsCustomFieldObjects()
    {
        var (client, handler) = ClientFactory.Create();
        handler.Enqueue(JsonPayloads.EntryPage("RPT1", ids: ["E1"]));

        var entries = await client.GetEntriesAsync("RPT1");

        Assert.NotNull(entries[0].Custom1);
        Assert.Equal("C1", entries[0].Custom1!.Code);
        Assert.Equal("Custom Value 1", entries[0].Custom1!.Value);
    }

    [Fact]
    public async Task GetEntries_DefaultsUserToAll()
    {
        var (client, handler) = ClientFactory.Create();
        handler.Enqueue(JsonPayloads.EmptyPage());

        await client.GetEntriesAsync("RPT1");

        var query = HttpUtility.ParseQueryString(handler.Requests[0].RequestUri!.Query);
        Assert.Equal("ALL", query["user"]);
    }

    [Fact]
    public async Task GetEntries_ReportIdAppearsInQuery()
    {
        var (client, handler) = ClientFactory.Create();
        handler.Enqueue(JsonPayloads.EmptyPage());

        await client.GetEntriesAsync("RPT-XYZ");

        var query = HttpUtility.ParseQueryString(handler.Requests[0].RequestUri!.Query);
        Assert.Equal("RPT-XYZ", query["reportID"]);
    }

    [Fact]
    public async Task GetEntries_ThrowsForNullReportId()
    {
        var (client, _) = ClientFactory.Create();
        await Assert.ThrowsAsync<ArgumentNullException>(() => client.GetEntriesAsync(null!));
    }

    [Fact]
    public async Task GetEntries_ThrowsForEmptyReportId()
    {
        var (client, _) = ClientFactory.Create();
        await Assert.ThrowsAsync<ArgumentException>(() => client.GetEntriesAsync(string.Empty));
    }

    [Fact]
    public async Task GetEntries_PaginatesThroughAllPages()
    {
        var (client, handler) = ClientFactory.Create(baseUrl: "https://api.example.com");
        handler.Enqueue(JsonPayloads.EntryPage("RPT1",
            nextPage: "https://api.example.com/api/v3.0/expense/entries?reportID=RPT1&offset=25",
            ids: ["E1"]));
        handler.Enqueue(JsonPayloads.EntryPage("RPT1", ids: ["E2"]));

        var entries = await client.GetEntriesAsync("RPT1");

        Assert.Equal(2, entries.Count);
        Assert.Equal(2, handler.Requests.Count);
    }

    // ── GetItemizationsAsync ─────────────────────────────────────────────────

    [Fact]
    public async Task GetItemizations_WithoutEntryId_IncludesOnlyReportIdInQuery()
    {
        var (client, handler) = ClientFactory.Create();
        handler.Enqueue(JsonPayloads.ItemizationPage("E1", "RPT1", ids: ["ITM1"]));

        await client.GetItemizationsAsync("RPT1");

        var query = HttpUtility.ParseQueryString(handler.Requests[0].RequestUri!.Query);
        Assert.Equal("RPT1", query["reportID"]);
        Assert.Null(query["entryID"]);
        Assert.Single(handler.Requests);
    }

    [Fact]
    public async Task GetItemizations_WithEntryId_IncludesEntryIdInQuery()
    {
        var (client, handler) = ClientFactory.Create();
        handler.Enqueue(JsonPayloads.ItemizationPage("E1", "RPT1", ids: ["ITM1"]));

        await client.GetItemizationsAsync("RPT1", entryId: "E1");

        var query = HttpUtility.ParseQueryString(handler.Requests[0].RequestUri!.Query);
        Assert.Equal("RPT1", query["reportID"]);
        Assert.Equal("E1", query["entryID"]);
        Assert.Single(handler.Requests);
    }

    [Fact]
    public async Task GetItemizations_ReturnsMappedItems()
    {
        var (client, handler) = ClientFactory.Create();
        handler.Enqueue(JsonPayloads.ItemizationPage("E1", "RPT1", ids: ["ITM1", "ITM2"]));

        var items = await client.GetItemizationsAsync("RPT1");

        Assert.Equal(2, items.Count);
        Assert.Equal("ITM1", items[0].ID);
        Assert.Equal("E1", items[0].EntryID);
        Assert.Equal("RPT1", items[0].ReportID);
    }

    [Fact]
    public async Task GetItemizations_MapsCustomFieldObjects()
    {
        var (client, handler) = ClientFactory.Create();
        handler.Enqueue(JsonPayloads.ItemizationPage("E1", "RPT1", ids: ["ITM1"]));

        var items = await client.GetItemizationsAsync("RPT1");

        Assert.NotNull(items[0].Custom1);
        Assert.Equal("C1", items[0].Custom1!.Code);
        Assert.Equal("Item Value", items[0].Custom1!.Value);
    }

    [Fact]
    public async Task GetItemizations_DefaultsUserToAll()
    {
        var (client, handler) = ClientFactory.Create();
        handler.Enqueue(JsonPayloads.EmptyPage());

        await client.GetItemizationsAsync("RPT1");

        var query = HttpUtility.ParseQueryString(handler.Requests[0].RequestUri!.Query);
        Assert.Equal("ALL", query["user"]);
    }

    [Fact]
    public async Task GetItemizations_ThrowsForNullReportId()
    {
        var (client, _) = ClientFactory.Create();
        await Assert.ThrowsAsync<ArgumentNullException>(() => client.GetItemizationsAsync(null!));
    }

    // ── GetAllocationsAsync ──────────────────────────────────────────────────

    [Fact]
    public async Task GetAllocations_ReturnsMappedItems()
    {
        var (client, handler) = ClientFactory.Create();
        handler.Enqueue(JsonPayloads.AllocationPage(ids: ["A1", "A2"]));

        var allocs = await client.GetAllocationsAsync("RPT1");

        Assert.Equal(2, allocs.Count);
        Assert.Equal("A1", allocs[0].ID);
        Assert.Equal("entry-1", allocs[0].EntryID);
        Assert.Equal("100.00", allocs[0].Percentage);
    }

    [Fact]
    public async Task GetAllocations_MapsCustomFieldObjects()
    {
        var (client, handler) = ClientFactory.Create();
        handler.Enqueue(JsonPayloads.AllocationPage(ids: ["A1"]));

        var allocs = await client.GetAllocationsAsync("RPT1");

        Assert.NotNull(allocs[0].Custom1);
        Assert.Equal("GL1", allocs[0].Custom1!.Code);
        Assert.Equal("GL Account", allocs[0].Custom1!.Value);
    }

    [Fact]
    public async Task GetAllocations_IncludesReportIdInQuery()
    {
        var (client, handler) = ClientFactory.Create();
        handler.Enqueue(JsonPayloads.EmptyPage());

        await client.GetAllocationsAsync("RPT-ABC");

        var query = HttpUtility.ParseQueryString(handler.Requests[0].RequestUri!.Query);
        Assert.Equal("RPT-ABC", query["reportID"]);
    }

    [Fact]
    public async Task GetAllocations_DefaultsUserToAll()
    {
        var (client, handler) = ClientFactory.Create();
        handler.Enqueue(JsonPayloads.EmptyPage());

        await client.GetAllocationsAsync("RPT1");

        var query = HttpUtility.ParseQueryString(handler.Requests[0].RequestUri!.Query);
        Assert.Equal("ALL", query["user"]);
    }

    [Fact]
    public async Task GetAllocations_PaginatesThroughAllPages()
    {
        var (client, handler) = ClientFactory.Create(baseUrl: "https://api.example.com");
        handler.Enqueue(JsonPayloads.AllocationPage(
            nextPage: "https://api.example.com/api/v3.0/expense/allocations?offset=25",
            ids: ["A1"]));
        handler.Enqueue(JsonPayloads.AllocationPage(ids: ["A2"]));

        var allocs = await client.GetAllocationsAsync("RPT1");

        Assert.Equal(2, allocs.Count);
        Assert.Equal(2, handler.Requests.Count);
    }

    [Fact]
    public async Task GetAllocations_WithEntryId_IncludesEntryIdInQuery()
    {
        var (client, handler) = ClientFactory.Create();
        handler.Enqueue(JsonPayloads.EmptyPage());

        await client.GetAllocationsAsync("RPT1", entryId: "E1");

        var query = HttpUtility.ParseQueryString(handler.Requests[0].RequestUri!.Query);
        Assert.Equal("E1", query["entryID"]);
    }

    [Fact]
    public async Task GetAllocations_WithItemizationId_IncludesItemizationIdInQuery()
    {
        var (client, handler) = ClientFactory.Create();
        handler.Enqueue(JsonPayloads.EmptyPage());

        await client.GetAllocationsAsync("RPT1", itemizationId: "ITM1");

        var query = HttpUtility.ParseQueryString(handler.Requests[0].RequestUri!.Query);
        Assert.Equal("ITM1", query["itemizationID"]);
    }

    [Fact]
    public async Task GetAllocations_WithoutOptionalIds_OmitsEntryAndItemizationFromQuery()
    {
        var (client, handler) = ClientFactory.Create();
        handler.Enqueue(JsonPayloads.EmptyPage());

        await client.GetAllocationsAsync("RPT1");

        var query = HttpUtility.ParseQueryString(handler.Requests[0].RequestUri!.Query);
        Assert.Null(query["entryID"]);
        Assert.Null(query["itemizationID"]);
    }

    [Fact]
    public async Task GetAllocations_ThrowsForNullReportId()
    {
        var (client, _) = ClientFactory.Create();
        await Assert.ThrowsAsync<ArgumentNullException>(() => client.GetAllocationsAsync(null!));
    }

    [Fact]
    public async Task GetAllocations_ThrowsForEmptyReportId()
    {
        var (client, _) = ClientFactory.Create();
        await Assert.ThrowsAsync<ArgumentException>(() => client.GetAllocationsAsync(string.Empty));
    }
}
