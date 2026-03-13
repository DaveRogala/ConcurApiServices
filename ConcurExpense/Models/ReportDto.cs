using System.Text.Json.Serialization;

namespace ConcurExpense.Models;

public record ReportDto : ExpenseBaseDto
{
    [JsonPropertyName("Name")]
    public required string Name { get; init; }

    [JsonPropertyName("Total")]
    public decimal? Total { get; init; }

    [JsonPropertyName("AmountDueCompanyCard")]
    public decimal? AmountDueCompanyCard { get; init; }

    [JsonPropertyName("AmountDueEmployee")]
    public decimal? AmountDueEmployee { get; init; }

    [JsonPropertyName("Country")]
    public string? Country { get; init; }

    [JsonPropertyName("CountrySubdivision")]
    public string? CountrySubdivision { get; init; }

    [JsonPropertyName("CreateDate")]
    public DateTime? CreateDate { get; init; }

    [JsonPropertyName("SubmitDate")]
    public DateTime? SubmitDate { get; init; }

    [JsonPropertyName("ProcessingPaymentDate")]
    public DateTime? ProcessingPaymentDate { get; init; }

    [JsonPropertyName("PaidDate")]
    public DateTime? PaidDate { get; init; }

    [JsonPropertyName("ApprovalStatusCode")]
    public string? ApprovalStatusCode { get; init; }

    [JsonPropertyName("ApprovalStatusName")]
    public string? ApprovalStatusName { get; init; }

    [JsonPropertyName("PaymentStatusCode")]
    public string? PaymentStatusCode { get; init; }

    [JsonPropertyName("PaymentStatusName")]
    public string? PaymentStatusName { get; init; }

    [JsonPropertyName("LastModifiedDate")]
    public DateTime? LastModifiedDate { get; init; }

    [JsonPropertyName("OwnerLoginID")]
    public string? OwnerLoginID { get; init; }

    [JsonPropertyName("OwnerName")]
    public string? OwnerName { get; init; }

    [JsonPropertyName("PolicyID")]
    public string? PolicyID { get; init; }

    [JsonPropertyName("PolicyName")]
    public string? PolicyName { get; init; }

    [JsonPropertyName("PurposeJustification")]
    public string? PurposeJustification { get; init; }

    [JsonPropertyName("ReportDate")]
    public DateTime? ReportDate { get; init; }

    [JsonPropertyName("HasException")]
    public bool? HasException { get; init; }

    [JsonPropertyName("HasImages")]
    public bool? HasImages { get; init; }

    [JsonPropertyName("LedgerName")]
    public string? LedgerName { get; init; }

    [JsonPropertyName("OrgUnit1")]
    public CustomFieldDto? OrgUnit1 { get; init; }

    [JsonPropertyName("OrgUnit2")]
    public CustomFieldDto? OrgUnit2 { get; init; }

    [JsonPropertyName("OrgUnit3")]
    public CustomFieldDto? OrgUnit3 { get; init; }

    [JsonPropertyName("OrgUnit4")]
    public CustomFieldDto? OrgUnit4 { get; init; }

    [JsonPropertyName("OrgUnit5")]
    public CustomFieldDto? OrgUnit5 { get; init; }

    [JsonPropertyName("OrgUnit6")]
    public CustomFieldDto? OrgUnit6 { get; init; }

    [JsonPropertyName("WorkflowActionURL")]
    public string? WorkflowActionURL { get; init; }
}
