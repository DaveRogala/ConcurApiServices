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
    /// Retrieves all expense entries for the given report, automatically paging through results.
    /// </summary>
    Task<List<EntryDto>> GetEntriesAsync(
        string reportId,
        int? limit = null,
        string? user = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all itemizations for all entries within the given report, automatically paging through results.
    /// </summary>
    Task<List<ItemizationDto>> GetItemizationsAsync(
        string reportId,
        int? limit = null,
        string? user = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all allocations for the given report, automatically paging through results.
    /// </summary>
    Task<List<AllocationDto>> GetAllocationsAsync(
        string reportId,
        int? limit = null,
        string? user = null,
        CancellationToken cancellationToken = default);
}
