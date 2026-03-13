using System.Text.Json.Serialization;

namespace ConcurExpense.Models;

public record AllocationDto : ExpenseBaseDto
{
    [JsonPropertyName("EntryID")]
    public string? EntryID { get; init; }

    [JsonPropertyName("ReportID")]
    public string? ReportID { get; init; }

    [JsonPropertyName("Percentage")]
    public decimal? Percentage { get; init; }

    [JsonPropertyName("Amount")]
    public decimal? Amount { get; init; }

    [JsonPropertyName("IsHidden")]
    public bool? IsHidden { get; init; }

    [JsonPropertyName("IsPercentEdited")]
    public bool? IsPercentEdited { get; init; }

    [JsonPropertyName("AccountCode1")]
    public string? AccountCode1 { get; init; }

    [JsonPropertyName("AccountCode2")]
    public string? AccountCode2 { get; init; }

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

    [JsonPropertyName("Custom6")]
    public CustomFieldDto? Custom6 { get; init; }

    [JsonPropertyName("Custom7")]
    public CustomFieldDto? Custom7 { get; init; }

    [JsonPropertyName("Custom8")]
    public CustomFieldDto? Custom8 { get; init; }

    [JsonPropertyName("Custom9")]
    public CustomFieldDto? Custom9 { get; init; }

    [JsonPropertyName("Custom10")]
    public CustomFieldDto? Custom10 { get; init; }

    [JsonPropertyName("Custom11")]
    public CustomFieldDto? Custom11 { get; init; }

    [JsonPropertyName("Custom12")]
    public CustomFieldDto? Custom12 { get; init; }

    [JsonPropertyName("Custom13")]
    public CustomFieldDto? Custom13 { get; init; }

    [JsonPropertyName("Custom14")]
    public CustomFieldDto? Custom14 { get; init; }

    [JsonPropertyName("Custom15")]
    public CustomFieldDto? Custom15 { get; init; }

    [JsonPropertyName("Custom16")]
    public CustomFieldDto? Custom16 { get; init; }

    [JsonPropertyName("Custom17")]
    public CustomFieldDto? Custom17 { get; init; }

    [JsonPropertyName("Custom18")]
    public CustomFieldDto? Custom18 { get; init; }

    [JsonPropertyName("Custom19")]
    public CustomFieldDto? Custom19 { get; init; }

    [JsonPropertyName("Custom20")]
    public CustomFieldDto? Custom20 { get; init; }
}
