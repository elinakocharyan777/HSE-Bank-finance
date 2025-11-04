using HSEBank.Logic.Domain.Enums;

namespace HSEBank.Logic.Application.Importing;

public class OperationImportRow
{
    public FinanceType Type { get; set; }
    public string Category { get; set; } = "";
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public string? Description { get; set; }
}