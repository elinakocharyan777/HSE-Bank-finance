using HSEBank.Logic.Domain.Enums;

namespace HSEBank.Logic.Application.Exporting;

public sealed class OperationExportContext
{
    public Guid OperationId { get; init; }
    public FinanceType Type { get; init; }
    public string CategoryName { get; init; } = "";
    public decimal Amount { get; init; }
    public DateTime Date { get; init; }
    public string? Description { get; init; }
}