namespace ConcurExpense.Models;

public record ReportDto : ExpenseBaseDto
{
    // Required
    public required string Name { get; init; }
    public required string CurrencyCode { get; init; }

    // Read-only / optional fields
    public decimal AmountDueCompanyCard { get; init; }
    public decimal AmountDueEmployee { get; init; }
    public string? ApprovalStatusCode { get; init; }
    public string? ApprovalStatusName { get; init; }
    public string? ApproverLoginID { get; init; }
    public string? ApproverName { get; init; }
    public string? Country { get; init; }
    public string? CountrySubdivision { get; init; }
    public DateTime? CreateDate { get; init; }
    public bool EverSentBack { get; init; }
    public bool HasException { get; init; }
    public string? LastComment { get; init; }
    public DateTime? LastModifiedDate { get; init; }
    public string? LedgerName { get; init; }
    public string? OwnerLoginID { get; init; }
    public string? OwnerName { get; init; }
    public DateTime? PaidDate { get; init; }
    public string? PaymentStatusCode { get; init; }
    public string? PaymentStatusName { get; init; }
    public decimal PersonalAmount { get; init; }
    public string? PolicyID { get; init; }
    public DateTime? ProcessingPaymentDate { get; init; }
    public bool ReceiptsReceived { get; init; }
    public DateTime? SubmitDate { get; init; }
    public decimal Total { get; init; }
    public decimal TotalApprovedAmount { get; init; }
    public decimal TotalClaimedAmount { get; init; }
    public DateTime? UserDefinedDate { get; init; }
    public string? WorkflowActionUrl { get; init; }

    // Org unit fields
    public CustomFieldDto? OrgUnit1 { get; init; }
    public CustomFieldDto? OrgUnit2 { get; init; }
    public CustomFieldDto? OrgUnit3 { get; init; }
    public CustomFieldDto? OrgUnit4 { get; init; }
    public CustomFieldDto? OrgUnit5 { get; init; }
    public CustomFieldDto? OrgUnit6 { get; init; }

    // Custom fields
    public CustomFieldDto? Custom1 { get; init; }
    public CustomFieldDto? Custom2 { get; init; }
    public CustomFieldDto? Custom3 { get; init; }
    public CustomFieldDto? Custom4 { get; init; }
    public CustomFieldDto? Custom5 { get; init; }
    public CustomFieldDto? Custom6 { get; init; }
    public CustomFieldDto? Custom7 { get; init; }
    public CustomFieldDto? Custom8 { get; init; }
    public CustomFieldDto? Custom9 { get; init; }
    public CustomFieldDto? Custom10 { get; init; }
    public CustomFieldDto? Custom11 { get; init; }
    public CustomFieldDto? Custom12 { get; init; }
    public CustomFieldDto? Custom13 { get; init; }
    public CustomFieldDto? Custom14 { get; init; }
    public CustomFieldDto? Custom15 { get; init; }
    public CustomFieldDto? Custom16 { get; init; }
    public CustomFieldDto? Custom17 { get; init; }
    public CustomFieldDto? Custom18 { get; init; }
    public CustomFieldDto? Custom19 { get; init; }
    public CustomFieldDto? Custom20 { get; init; }
}
