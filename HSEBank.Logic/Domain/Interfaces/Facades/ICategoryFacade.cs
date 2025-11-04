using HSEBank.Logic.Domain.Entities;
using HSEBank.Logic.Domain.Enums;

namespace HSEBank.Logic.Domain.Interfaces.Facades;

public interface ICategoryFacade
{
    Guid CreateCategory(string name, FinanceType type);
    void RenameCategory(Guid categoryId, string newName);
    bool DeleteCategory(Guid categoryId);
    Category? GetById(Guid categoryId);
    IReadOnlyCollection<Category> GetAll();
}