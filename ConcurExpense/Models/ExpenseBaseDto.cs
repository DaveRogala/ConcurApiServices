namespace ConcurExpense.Models;

public abstract record ExpenseBaseDto
{
    public string? ID { get; init; }
    public string? URI { get; init; }
}
