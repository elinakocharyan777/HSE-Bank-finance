using HSEBank.Logic.Domain.Entities;
using HSEBank.Logic.Domain.Enums;
using HSEBank.Logic.Domain.Interfaces;

namespace HSEBank.Logic.Application.Exporting;

public class OperationExporter
{
    private readonly IRepository<Operation> _ops;
    private readonly IRepository<Category> _cats;

    public OperationExporter(IRepository<Operation> ops, IRepository<Category> cats)
    {
        _ops = ops;
        _cats = cats;
    }

    public string ExportForAccount(Guid accountId, IOperationExportVisitor visitor,
        DateTime? from = null, DateTime? to = null)
    {
        var catIndex = _cats.GetAll().ToDictionary(c => c.Id, c => (c.Name, c.Type));
        var items = _ops.GetAll()
            .Where(o => o.BankAccountId == accountId)
            .Where(o => from is null || o.Date >= from.Value)
            .Where(o => to is null || o.Date <= to.Value)
            .OrderBy(o => o.Date);

        visitor.Begin();

        foreach (var o in items)
        {
            var (catName, catType) = catIndex.TryGetValue(o.CategoryId, out var v)
                ? v : ("<unknown>", FinanceType.Expense);

            visitor.Visit(new OperationExportContext
            {
                OperationId = o.Id,
                Type = o.Type,
                CategoryName = catName,
                Amount = o.Amount,
                Date = o.Date,
                Description = o.Description
            });
        }

        return visitor.End();
    }
}