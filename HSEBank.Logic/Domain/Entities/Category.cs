using HSEBank.Logic.Domain.Interfaces;
using HSEBank.Logic.Domain.Enums;

namespace HSEBank.Logic.Domain.Entities;

public class Category : IEntity
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public FinanceType Type { get; private set; }
    public string Name { get; private set; }

    public Category(string name, FinanceType type)
    {
        Name = string.IsNullOrWhiteSpace(name) ? throw new ArgumentException("Name is required") : name;
        Type = type;
    }

    public void Rename(string newName)
    {
        Name = string.IsNullOrWhiteSpace(newName) ? throw new ArgumentException("Name is required") : newName;
    }
}