namespace ConcurExpense.Models;

/// <summary>
/// Full set of filter parameters accepted by the Concur Expense Entries v3 API.
/// All properties are optional; omitted properties are excluded from the request.
/// <c>User</c> defaults to <c>"ALL"</c> when not specified.
/// <c>Limit</c> must not exceed 100.
/// </summary>
public record EntryQueryOptions
{
    /// <summary>Filters entries by report ID.</summary>
    public string? ReportID { get; init; }

    /// <summary>Max records per page. Must not exceed 100.</summary>
    public int? Limit { get; init; }

    /// <summary>Report owner login ID, or <c>"ALL"</c>. Defaults to <c>"ALL"</c>.</summary>
    public string? User { get; init; }

    /// <summary>Filters entries by payment type identifier.</summary>
    public string? PaymentTypeID { get; init; }

    /// <summary>Filters entries by batch ID.</summary>
    public string? BatchID { get; init; }

    /// <summary>Filters entries by expense type code.</summary>
    public string? ExpenseTypeCode { get; init; }

    /// <summary>Filters entries by attendee type code.</summary>
    public string? AttendeeTypeCode { get; init; }

    /// <summary>Filters entries by a specific attendee ID.</summary>
    public string? AttendeeID { get; init; }

    /// <summary>Filters by presence of VAT details.</summary>
    public bool? HasVAT { get; init; }

    /// <summary>Filters by presence of attendees.</summary>
    public bool? HasAttendees { get; init; }

    /// <summary>Filters by the billable flag.</summary>
    public bool? IsBillable { get; init; }
}
