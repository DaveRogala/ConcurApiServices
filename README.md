# ConcurExpense

A .NET 8 class library for retrieving expense data from the [SAP Concur Expense v3 API](https://developer.concur.com/api-reference/). Supports Reports, Entries, Itemizations, and Allocations with automatic pagination and OAuth2 token management.

## Features

- Retrieves all pages of results automatically using the `NextPage` field
- OAuth2 `refresh_token` grant with in-memory token caching
- Optional HTTP proxy support with credentials
- Strongly-typed record DTOs including nested `CustomFieldDto` for `OrgUnit*` and `Custom*` fields
- Designed for dependency injection via `IServiceCollection`

## Installation

Add the `ConcurExpense` project reference to your solution, or reference the compiled assembly.

## Configuration

Register the client in your DI setup by calling `AddConcurExpenseClient` on your `IServiceCollection`:

```csharp
builder.Services.AddConcurExpenseClient(options =>
{
    options.BaseUrl       = "https://us.api.concursolutions.com"; // default
    options.ClientId      = "your-client-id";
    options.ClientSecret  = "your-client-secret";
    options.RefreshToken  = "your-refresh-token";
});
```

### Configuration via `appsettings.json`

You can bind options from configuration instead:

```json
{
  "ConcurExpense": {
    "BaseUrl":       "https://us.api.concursolutions.com",
    "ClientId":      "your-client-id",
    "ClientSecret":  "your-client-secret",
    "RefreshToken":  "your-refresh-token"
  }
}
```

```csharp
builder.Services.AddConcurExpenseClient(options =>
    builder.Configuration.GetSection(ConcurOptions.SectionName).Bind(options));
```

### Proxy support

If your environment requires an HTTP proxy, set the proxy options:

```csharp
builder.Services.AddConcurExpenseClient(options =>
{
    options.ClientId     = "your-client-id";
    options.ClientSecret = "your-client-secret";
    options.RefreshToken = "your-refresh-token";

    options.ProxyUrl      = "http://proxy.example.com:8080";
    options.ProxyUsername = "proxyuser";   // omit if proxy has no auth
    options.ProxyPassword = "proxypass";   // omit if proxy has no auth
});
```

## Usage

Inject `IConcurExpenseClient` into your service or controller:

```csharp
public class ExpenseService(IConcurExpenseClient concur)
{
    public async Task<List<ReportDto>> GetRecentApprovedReports()
    {
        return await concur.GetReportsAsync(
            modifiedDateAfter:  DateTime.Today.AddDays(-30),
            approvalStatusCode: "A_APPR");
    }
}
```

### GetReportsAsync

```csharp
List<ReportDto> reports = await concur.GetReportsAsync(
    limit:              100,               // page size (optional)
    modifiedDateAfter:  DateTime.Today.AddDays(-7),   // optional
    modifiedDateBefore: DateTime.Today,               // optional
    user:               "jane@example.com",           // defaults to "ALL"
    approvalStatusCode: "A_APPR");                    // optional
```

| Parameter | Type | Description |
|---|---|---|
| `limit` | `int?` | Max records per page |
| `modifiedDateAfter` | `DateTime?` | Only reports modified after this date |
| `modifiedDateBefore` | `DateTime?` | Only reports modified before this date |
| `user` | `string?` | Concur login ID; defaults to `ALL` (all users) |
| `approvalStatusCode` | `string?` | e.g. `A_APPR`, `A_PEND` |

### GetEntriesAsync

```csharp
List<EntryDto> entries = await concur.GetEntriesAsync(
    reportId: "ABC123",
    limit:    100,
    user:     "jane@example.com");
```

### GetItemizationsAsync

Fetches all entries for the report, then retrieves itemizations for each itemized entry. All itemizations are returned in a single flat list.

```csharp
List<ItemizationDto> itemizations = await concur.GetItemizationsAsync(
    reportId: "ABC123",
    limit:    100,
    user:     "jane@example.com");
```

### GetAllocationsAsync

```csharp
List<AllocationDto> allocations = await concur.GetAllocationsAsync(
    reportId: "ABC123",
    limit:    100,
    user:     "jane@example.com");
```

## DTO Reference

All methods return lists of immutable `record` types. `OrgUnit*` and `Custom*` fields are `CustomFieldDto?` objects:

```csharp
public record CustomFieldDto(
    string? Code,
    string? Value,
    string? Type,
    string? ListItemID
);
```

Key DTO types: `ReportDto`, `EntryDto`, `ItemizationDto`, `AllocationDto`.

## ConcurOptions Reference

| Property | Type | Default | Description |
|---|---|---|---|
| `BaseUrl` | `string` | `https://us.api.concursolutions.com` | API base URL |
| `ClientId` | `string` | — | OAuth2 client ID |
| `ClientSecret` | `string` | — | OAuth2 client secret |
| `RefreshToken` | `string` | — | OAuth2 refresh token |
| `ProxyUrl` | `string?` | `null` | Proxy URL (e.g. `http://proxy:8080`) |
| `ProxyUsername` | `string?` | `null` | Proxy username (if required) |
| `ProxyPassword` | `string?` | `null` | Proxy password (if required) |

## License

[MIT](LICENSE)
