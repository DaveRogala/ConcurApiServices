using System.Text.Json.Serialization;

namespace ConcurExpense.Models;

public record ItemizationDto : ExpenseBaseDto
{
    [JsonPropertyName("EntryID")]
    public required string EntryID { get; init; }

    [JsonPropertyName("ReportID")]
    public required string ReportID { get; init; }

    [JsonPropertyName("ExpenseTypeCode")]
    public required string ExpenseTypeCode { get; init; }

    [JsonPropertyName("ExpenseTypeName")]
    public required string ExpenseTypeName { get; init; }

    [JsonPropertyName("ReportOwnerID")]
    public required string ReportOwnerID { get; init; }

    [JsonPropertyName("SpendCategoryCode")]
    public required string SpendCategoryCode { get; init; }

    [JsonPropertyName("SpendCategoryName")]
    public required string SpendCategoryName { get; init; }

    [JsonPropertyName("TransactionDate")]
    public required DateTime TransactionDate { get; init; }

    [JsonPropertyName("BusinessPurpose")]
    public string? BusinessPurpose { get; init; }

    [JsonPropertyName("TransactionAmount")]
    public decimal? TransactionAmount { get; init; }

    [JsonPropertyName("PostedAmount")]
    public decimal? PostedAmount { get; init; }

    [JsonPropertyName("VendorDescription")]
    public string? VendorDescription { get; init; }

    [JsonPropertyName("LocationName")]
    public string? LocationName { get; init; }

    [JsonPropertyName("LocationCountry")]
    public string? LocationCountry { get; init; }

    [JsonPropertyName("LocationSubdivision")]
    public string? LocationSubdivision { get; init; }

    [JsonPropertyName("IsPersonal")]
    public bool? IsPersonal { get; init; }

    [JsonPropertyName("HasImage")]
    public bool? HasImage { get; init; }

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
