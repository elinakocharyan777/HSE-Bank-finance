namespace HSEBank.Logic.Application.Analytics;

public sealed class CategoryTotal
{
    public string CategoryName { get; init; } = "";
    public decimal Total { get; init; } // знак уже учтён (+ для доходов, − для расходов)
}