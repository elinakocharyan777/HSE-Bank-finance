using HSEBank.Logic.Domain.Entities;
using HSEBank.Logic.Domain.Enums;
using HSEBank.Logic.Domain.Interfaces;

namespace HSEBank.Logic.Application.Analytics;

public class AnalyticsService : IAnalyticsService
{
    private readonly IRepository<Operation> _ops;
    private readonly IRepository<Category> _cats;

    public AnalyticsService(IRepository<Operation> ops, IRepository<Category> cats)
    {
        _ops = ops;
        _cats = cats;
    }

    public TypeTotals GetTypeTotals(Guid accountId, DateTime from, DateTime to)
    {
        var items = Filter(accountId, from, to);
        var income = items.Where(o => o.Type == FinanceType.Income).Sum(o => o.Amount);
        var expense = items.Where(o => o.Type == FinanceType.Expense).Sum(o => o.Amount);
        return new TypeTotals { Income = income, Expense = expense };
    }

    public IReadOnlyList<CategoryTotal> GetTotalsByCategory(Guid accountId, DateTime from, DateTime to)
    {
        var catIndex = _cats.GetAll().ToDictionary(c => c.Id, c => (c.Name, c.Type));
        var items = Filter(accountId, from, to);

        var grouped = items
            .GroupBy(o => o.CategoryId)
            .Select(g =>
            {
                var (name, type) = catIndex.TryGetValue(g.Key, out var v)
                    ? v : ("<unknown>", FinanceType.Expense);

                var sum = g.Sum(o => o.Amount) * (type == FinanceType.Expense ? -1 : 1);
                return new CategoryTotal { CategoryName = name, Total = sum };
            })
            .OrderByDescending(x => x.Total)
            .ToList();

        return grouped;
    }

    private IEnumerable<Operation> Filter(Guid accountId, DateTime from, DateTime to) =>
        _ops.GetAll().Where(o => o.BankAccountId == accountId && o.Date >= from && o.Date <= to);
}