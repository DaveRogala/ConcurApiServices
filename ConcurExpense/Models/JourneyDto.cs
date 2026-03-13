using System.Text.Json.Serialization;

namespace ConcurExpense.Models;

public record JourneyDto
{
    [JsonPropertyName("StartLocation")]
    public required string StartLocation { get; init; }

    [JsonPropertyName("EndLocation")]
    public required string EndLocation { get; init; }

    [JsonPropertyName("UnitOfMeasure")]
    public required string UnitOfMeasure { get; init; }

    [JsonPropertyName("OdometerStart")]
    public decimal? OdometerStart { get; init; }

    [JsonPropertyName("OdometerEnd")]
    public decimal? OdometerEnd { get; init; }

    [JsonPropertyName("BusinessDistance")]
    public decimal? BusinessDistance { get; init; }

    [JsonPropertyName("PersonalDistance")]
    public decimal? PersonalDistance { get; init; }

    [JsonPropertyName("NumberOfPassengers")]
    public int? NumberOfPassengers { get; init; }
}
