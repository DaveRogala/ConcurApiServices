using ConcurExpense.Models;

namespace ConcurExpense;

public interface IConcurExpenseClient
{
    /// <summary>
    /// Retrieves all expense reports matching the given filters, automatically paging through results.
    /// </summary>
    Task<List<ReportDto>> GetReportsAsync(
        int? limit = null,
        DateTime? modifiedDateBefore = null,
        DateTime? modifiedDateAfter = null,
        string? user = null,
        string? approvalStatusCode = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all expense reports matching the given filters, automatically paging through results.
    /// Accepts the full set of query parameters supported by the Concur Reports v3 API.
    /// </summary>
    Task<List<ReportDto>> GetReportsAsync(
        ReportQueryOptions options,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all expense entries for the given report, automatically paging through results.
    /// </summary>
    Task<List<EntryDto>> GetEntriesAsync(
        string? reportId,
        int? limit = null,
        string? user = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves expense entries matching the given filters, automatically paging through results.
    /// Accepts the full set of query parameters supported by the Concur Entries v3 API.
    /// <paramref name="options"/>.<c>Limit</c> must not exceed 100.
    /// </summary>
    Task<List<EntryDto>> GetEntriesAsync(
        EntryQueryOptions options,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves itemizations, automatically paging through results.
    /// If <paramref name="entryId"/> is provided, retrieves itemizations for that specific entry only.
    /// Otherwise retrieves all itemizations for the report.
    /// </summary>
    Task<List<ItemizationDto>> GetItemizationsAsync(
        string reportId,
        string? entryId = null,
        int? limit = null,
        string? user = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all allocations for the given report, automatically paging through results.
    /// Optionally filter by <paramref name="entryId"/> or <paramref name="itemizationId"/>.
    /// </summary>
    Task<List<AllocationDto>> GetAllocationsAsync(
        string reportId,
        string? entryId = null,
        string? itemizationId = null,
        int? limit = null,
        string? user = null,
        CancellationToken cancellationToken = default);
}
