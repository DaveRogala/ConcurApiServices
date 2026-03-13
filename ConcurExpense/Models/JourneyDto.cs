namespace ConcurExpense.Models;

public record JourneyDto
{
    public required string StartLocation { get; init; }
    public required string EndLocation { get; init; }
    public required string UnitOfMeasure { get; init; }
    public decimal? OdometerStart { get; init; }
    public decimal? OdometerEnd { get; init; }
    public decimal? BusinessDistance { get; init; }
    public decimal? PersonalDistance { get; init; }
    public int? NumberOfPassengers { get; init; }
}
