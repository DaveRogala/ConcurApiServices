using System.Text.Json.Serialization;

namespace ConcurExpense.Models;

public record CustomFieldDto(
    [property: JsonPropertyName("Code")] string? Code,
    [property: JsonPropertyName("Value")] string? Value,
    [property: JsonPropertyName("Type")] string? Type,
    [property: JsonPropertyName("ListItemID")] string? ListItemID
);
