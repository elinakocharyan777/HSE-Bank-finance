using HSEBank.Logic.Domain.Entities;
using HSEBank.Logic.Domain.Enums;
using HSEBank.Logic.Domain.Interfaces.Factories;

namespace HSEBank.Logic.Domain.Factories;

public class CategoryFactory : ICategoryFactory
{
    public Category Create(string name, FinanceType type) => new(name, type);
}