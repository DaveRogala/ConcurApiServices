using System.Text.Json.Serialization;

namespace ConcurExpense.Models;

public abstract record ExpenseBaseDto(
    [property: JsonPropertyName("ID")] string? ID,
    [property: JsonPropertyName("CurrencyCode")] string? CurrencyCode,
    [property: JsonPropertyName("URI")] string? URI
);
