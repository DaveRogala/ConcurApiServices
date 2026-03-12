using System.Text.Json.Serialization;

namespace ConcurExpense.Models;

public record ItemizationDto(
    [property: JsonPropertyName("ID")] string? ID,
    [property: JsonPropertyName("EntryID")] string? EntryID,
    [property: JsonPropertyName("ReportID")] string? ReportID,
    [property: JsonPropertyName("ExpenseTypeCode")] string? ExpenseTypeCode,
    [property: JsonPropertyName("ExpenseTypeName")] string? ExpenseTypeName,
    [property: JsonPropertyName("BusinessPurpose")] string? BusinessPurpose,
    [property: JsonPropertyName("TransactionAmount")] decimal? TransactionAmount,
    [property: JsonPropertyName("PostedAmount")] decimal? PostedAmount,
    [property: JsonPropertyName("CurrencyCode")] string? CurrencyCode,
    [property: JsonPropertyName("TransactionDate")] DateTime? TransactionDate,
    [property: JsonPropertyName("VendorDescription")] string? VendorDescription,
    [property: JsonPropertyName("LocationName")] string? LocationName,
    [property: JsonPropertyName("LocationCountry")] string? LocationCountry,
    [property: JsonPropertyName("LocationSubdivision")] string? LocationSubdivision,
    [property: JsonPropertyName("IsPersonal")] bool? IsPersonal,
    [property: JsonPropertyName("HasImage")] bool? HasImage,
    [property: JsonPropertyName("Custom1")] CustomFieldDto? Custom1,
    [property: JsonPropertyName("Custom2")] CustomFieldDto? Custom2,
    [property: JsonPropertyName("Custom3")] CustomFieldDto? Custom3,
    [property: JsonPropertyName("Custom4")] CustomFieldDto? Custom4,
    [property: JsonPropertyName("Custom5")] CustomFieldDto? Custom5,
    [property: JsonPropertyName("URI")] string? URI
);
