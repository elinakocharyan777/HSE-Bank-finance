using HSEBank.Logic.Domain.Entities;
using HSEBank.Logic.Domain.Enums;
using HSEBank.Logic.Domain.Interfaces;
using HSEBank.Logic.Domain.Interfaces.Factories;
using HSEBank.Logic.Domain.Interfaces.Facades;

namespace HSEBank.Logic.Domain.Facades;

public class CategoryFacade : ICategoryFacade
{
    private readonly IRepository<Category> _repo;
    private readonly ICategoryFactory _factory;

    public CategoryFacade(IRepository<Category> repo, ICategoryFactory factory)
    {
        _repo = repo;
        _factory = factory;
    }

    public Guid CreateCategory(string name, FinanceType type)
    {
        var cat = _factory.Create(name, type);
        _repo.Add(cat);
        return cat.Id;
    }

    public void RenameCategory(Guid categoryId, string newName)
    {
        var cat = _repo.GetById(categoryId) ?? throw new InvalidOperationException("Category not found");
        cat.Rename(newName);
        _repo.Update(cat);
    }

    public bool DeleteCategory(Guid categoryId) => _repo.Remove(categoryId);
    public Category? GetById(Guid categoryId) => _repo.GetById(categoryId);
    public IReadOnlyCollection<Category> GetAll() => _repo.GetAll();
}