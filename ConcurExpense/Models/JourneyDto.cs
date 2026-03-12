using System.Text.Json.Serialization;

namespace ConcurExpense.Models;

public record JourneyDto(
    [property: JsonPropertyName("StartLocation")] string? StartLocation,
    [property: JsonPropertyName("EndLocation")] string? EndLocation,
    [property: JsonPropertyName("OdometerStart")] decimal? OdometerStart,
    [property: JsonPropertyName("OdometerEnd")] decimal? OdometerEnd,
    [property: JsonPropertyName("BusinessDistance")] decimal? BusinessDistance,
    [property: JsonPropertyName("PersonalDistance")] decimal? PersonalDistance,
    [property: JsonPropertyName("NumberOfPassengers")] int? NumberOfPassengers,
    [property: JsonPropertyName("UnitOfMeasure")] string? UnitOfMeasure
);
