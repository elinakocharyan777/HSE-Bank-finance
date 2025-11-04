namespace HSEBank.Logic.Application.Analytics;

public sealed class TypeTotals
{
    public decimal Income { get; init; }
    public decimal Expense { get; init; } // положительное число
    public decimal Net => Income - Expense;
}