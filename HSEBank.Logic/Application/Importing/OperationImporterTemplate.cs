using HSEBank.Logic.Application.Importing.Validation;
using HSEBank.Logic.Domain.Entities;
using HSEBank.Logic.Domain.Enums;
using HSEBank.Logic.Domain.Interfaces;
using HSEBank.Logic.Domain.Interfaces.Facades;
using HSEBank.Logic.Domain.Interfaces.Factories;

namespace HSEBank.Logic.Application.Importing;

/// <summary>
/// Template Method: Import -> Read -> Parse -> Validate -> Persist
/// Общая логика сохранения и валидации; парсинг делегируется наследникам.
/// </summary>
public abstract class OperationImporterTemplate : IOperationImporter
{
    protected readonly IOperationFacade _ops;
    protected readonly IRepository<BankAccount> _accounts;
    protected readonly IRepository<Category> _categories;
    protected readonly ICategoryFactory _categoryFactory;

    protected OperationImporterTemplate(
        IOperationFacade ops,
        IRepository<BankAccount> accounts,
        IRepository<Category> categories,
        ICategoryFactory categoryFactory)
    {
        _ops = ops;
        _accounts = accounts;
        _categories = categories;
        _categoryFactory = categoryFactory;
    }

    public abstract string Id { get; }
    public bool CanHandle(string pathOrExt)
    {
        var ext = Path.GetExtension(pathOrExt).ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(ext)) ext = pathOrExt.ToLowerInvariant();
        return ext == $".{Id}" || ext == Id;
    }

    public int Import(string filePath, Guid accountId)
    {
        if (_accounts.GetById(accountId) is null)
            throw new InvalidOperationException("Account not found");

        var content = File.ReadAllText(filePath);
        var rows = Parse(content).ToList();
        var valid = Validate(rows).ToList();

        var imported = 0;
        foreach (var r in valid)
        {
            var cat = EnsureCategory(r.Category, r.Type);
            _ops.AddOperation(r.Type, accountId, cat.Id, r.Amount, r.Date, r.Description);
            imported++;
        }
        return imported;
    }

    protected abstract IEnumerable<OperationImportRow> Parse(string content);

    protected virtual IEnumerable<OperationImportRow> Validate(IEnumerable<OperationImportRow> rows)
    {
        var root = ImportRulesFactory.BuildDefault();
        foreach (var r in rows)
            if (root.Validate(r))
                yield return r;
    }

    protected Category EnsureCategory(string name, FinanceType type)
    {
        var existing = _categories.GetAll()
            .FirstOrDefault(c => string.Equals(c.Name, name, StringComparison.OrdinalIgnoreCase)
                              && c.Type == type);
        if (existing != null) return existing;

        var created = _categoryFactory.Create(name, type);
        _categories.Add(created);
        return created;
    }
}