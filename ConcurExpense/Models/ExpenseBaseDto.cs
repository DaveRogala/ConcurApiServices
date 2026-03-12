using System.Text.Json.Serialization;

namespace ConcurExpense.Models;

public abstract record ExpenseBaseDto
{
    [JsonPropertyName("ID")]
    public string? ID { get; init; }

    [JsonPropertyName("CurrencyCode")]
    public string? CurrencyCode { get; init; }

    [JsonPropertyName("URI")]
    public string? URI { get; init; }
}
