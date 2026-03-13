namespace ConcurExpense.Models;

public record CustomFieldDto
{
    public string? Code { get; init; }
    public string? ListItemID { get; init; }
    public string? Type { get; init; }
    public string? Value { get; init; }
}
