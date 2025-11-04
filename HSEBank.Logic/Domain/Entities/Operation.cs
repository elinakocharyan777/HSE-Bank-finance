using HSEBank.Logic.Domain.Enums;
using HSEBank.Logic.Domain.Interfaces;

namespace HSEBank.Logic.Domain.Entities;

public class Operation : IEntity
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public FinanceType Type { get; private set; }
    public Guid BankAccountId { get; private set; }
    public Guid CategoryId { get; private set; }
    public decimal Amount { get; private set; }
    public DateTime Date { get; private set; }
    public string? Description { get; private set; }

    public Operation(FinanceType type, Guid bankAccountId, Guid categoryId, decimal amount, DateTime date, string? description = null)
    {
        if (amount <= 0) throw new ArgumentOutOfRangeException(nameof(amount), "Amount must be positive");

        Type = type;
        BankAccountId = bankAccountId;
        CategoryId = categoryId;
        Amount = amount;
        Date = date;
        Description = description;
    }
    public void Edit(FinanceType type, Guid categoryId, decimal amount, DateTime date, string? description)
    {
        if (amount <= 0) throw new ArgumentOutOfRangeException(nameof(amount), "Amount must be positive");
        Type = type;
        CategoryId = categoryId;
        Amount = amount;
        Date = date;
        Description = description;
    }
}