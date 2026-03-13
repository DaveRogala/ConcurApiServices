using System.Text.Json.Serialization;

namespace ConcurExpense.Models;

public record AllocationDto : ExpenseBaseDto
{
    // Required
    public required string EntryID { get; init; }
    public required bool IsHidden { get; init; }
    public required bool IsPercentEdited { get; init; }
    public required string Percentage { get; init; }

    // Optional fields
    public string? AccountCode2 { get; init; }
    [JsonPropertyName("AccountCode1")]
    public string? AccountNumber { get; init; }

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
