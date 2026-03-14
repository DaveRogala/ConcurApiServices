namespace ConcurExpense.Models;

/// <summary>
/// Full set of filter parameters accepted by the Concur Expense Reports v3 API.
/// All properties are optional; omitted properties are excluded from the request.
/// <c>User</c> defaults to <c>"ALL"</c> when not specified.
/// </summary>
public record ReportQueryOptions
{
    /// <summary>Max records per page.</summary>
    public int? Limit { get; init; }

    /// <summary>Report owner login ID, or <c>"ALL"</c>. Defaults to <c>"ALL"</c>.</summary>
    public string? User { get; init; }

    /// <summary>Filter by approval status code (e.g. <c>"A_APPR"</c>, <c>"A_PEND"</c>).</summary>
    public string? ApprovalStatusCode { get; init; }

    /// <summary>Filter by payment status code.</summary>
    public string? PaymentStatusCode { get; init; }

    /// <summary>ISO 4217 3-letter currency code for the report currency.</summary>
    public string? CurrencyCode { get; init; }

    /// <summary>Unique identifier for the payment type of expense entries.</summary>
    public string? PaymentType { get; init; }

    /// <summary>Reimbursement method (e.g. <c>"ADPPAYR"</c>, <c>"APCHECK"</c>, <c>"CNQRPAY"</c>, <c>"PMTSERV"</c>).</summary>
    public string? ReimbursementMethod { get; init; }

    /// <summary>Login ID of the current report approver.</summary>
    public string? ApproverLoginID { get; init; }

    /// <summary>Expense type code from expense group configurations.</summary>
    public string? ExpenseTypeCode { get; init; }

    /// <summary>Filters reports that have attendees of the specified type code.</summary>
    public string? AttendeeTypeCode { get; init; }

    /// <summary>ISO 3166-1 alpha-2 country code.</summary>
    public string? CountryCode { get; init; }

    /// <summary>Unique identifier for a payment batch.</summary>
    public string? BatchID { get; init; }

    /// <summary>Vendor description for expense entries.</summary>
    public string? VendorName { get; init; }

    /// <summary>Filters by presence of VAT details.</summary>
    public bool? HasVAT { get; init; }

    /// <summary>Filters by presence of entry or report images.</summary>
    public bool? HasImages { get; init; }

    /// <summary>Filters by presence of attendees.</summary>
    public bool? HasAttendees { get; init; }

    /// <summary>Filters by the billable expense flag.</summary>
    public bool? HasBillableExpenses { get; init; }

    /// <summary>Filters by test user status.</summary>
    public bool? IsTestUser { get; init; }

    /// <summary>Unique identifier for the expense group configuration.</summary>
    public string? ExpenseGroupConfigID { get; init; }

    /// <summary>Filters entries with a transaction date before this value.</summary>
    public DateTime? EntryTransactionDateBefore { get; init; }

    /// <summary>Filters entries with a transaction date after this value.</summary>
    public DateTime? EntryTransactionDateAfter { get; init; }

    /// <summary>Filters reports created before this date.</summary>
    public DateTime? CreateDateBefore { get; init; }

    /// <summary>Filters reports created after this date.</summary>
    public DateTime? CreateDateAfter { get; init; }

    /// <summary>Filters by user-defined date before this value.</summary>
    public DateTime? UserDefinedDateBefore { get; init; }

    /// <summary>Filters by user-defined date after this value.</summary>
    public DateTime? UserDefinedDateAfter { get; init; }

    /// <summary>Filters reports submitted before this date.</summary>
    public DateTime? SubmitDateBefore { get; init; }

    /// <summary>Filters reports submitted after this date.</summary>
    public DateTime? SubmitDateAfter { get; init; }

    /// <summary>Filters by processing payment date before this value.</summary>
    public DateTime? ProcessingPaymentDateBefore { get; init; }

    /// <summary>Filters by processing payment date after this value.</summary>
    public DateTime? ProcessingPaymentDateAfter { get; init; }

    /// <summary>Filters reports paid before this date.</summary>
    public DateTime? PaidDateBefore { get; init; }

    /// <summary>Filters reports paid after this date.</summary>
    public DateTime? PaidDateAfter { get; init; }

    /// <summary>Filters reports last modified before this date.</summary>
    public DateTime? ModifiedDateBefore { get; init; }

    /// <summary>Filters reports last modified after this date.</summary>
    public DateTime? ModifiedDateAfter { get; init; }
}
