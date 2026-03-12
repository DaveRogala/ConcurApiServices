using System.Text.Json.Serialization;

namespace ConcurExpense.Models;

public record EntryDto : ExpenseBaseDto
{
    [JsonPropertyName("ReportID")]
    public required string ReportID { get; init; }

    [JsonPropertyName("ExpenseTypeCode")]
    public required string ExpenseTypeCode { get; init; }

    [JsonPropertyName("TransactionAmount")]
    public required decimal TransactionAmount { get; init; }

    [JsonPropertyName("TransactionCurrencyCode")]
    public required string TransactionCurrencyCode { get; init; }

    [JsonPropertyName("TransactionDate")]
    public required DateTime TransactionDate { get; init; }

    [JsonPropertyName("PaymentTypeID")]
    public required string PaymentTypeID { get; init; }

    [JsonPropertyName("ExpenseTypeName")]
    public string? ExpenseTypeName { get; init; }

    [JsonPropertyName("BusinessPurpose")]
    public string? BusinessPurpose { get; init; }

    [JsonPropertyName("PostedAmount")]
    public decimal? PostedAmount { get; init; }

    [JsonPropertyName("ApprovedAmount")]
    public string? ApprovedAmount { get; init; }

    [JsonPropertyName("VendorDescription")]
    public string? VendorDescription { get; init; }

    [JsonPropertyName("LocationName")]
    public string? LocationName { get; init; }

    [JsonPropertyName("LocationCountry")]
    public string? LocationCountry { get; init; }

    [JsonPropertyName("LocationSubdivision")]
    public string? LocationSubdivision { get; init; }

    [JsonPropertyName("IsItemized")]
    public bool? IsItemized { get; init; }

    [JsonPropertyName("IsBillable")]
    public bool? IsBillable { get; init; }

    [JsonPropertyName("IsPersonal")]
    public bool? IsPersonal { get; init; }

    [JsonPropertyName("PaymentTypeCode")]
    public string? PaymentTypeCode { get; init; }

    [JsonPropertyName("PaymentTypeName")]
    public string? PaymentTypeName { get; init; }

    [JsonPropertyName("ReceiptRequired")]
    public string? ReceiptRequired { get; init; }

    [JsonPropertyName("HasImage")]
    public bool? HasImage { get; init; }

    [JsonPropertyName("HasVat")]
    public bool? HasVat { get; init; }

    [JsonPropertyName("Journey")]
    public JourneyDto? Journey { get; init; }

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

    [JsonPropertyName("Custom1")]
    public CustomFieldDto? Custom1 { get; init; }

    [JsonPropertyName("Custom2")]
    public CustomFieldDto? Custom2 { get; init; }

    [JsonPropertyName("Custom3")]
    public CustomFieldDto? Custom3 { get; init; }

    [JsonPropertyName("Custom4")]
    public CustomFieldDto? Custom4 { get; init; }

    [JsonPropertyName("Custom5")]
    public CustomFieldDto? Custom5 { get; init; }
}
