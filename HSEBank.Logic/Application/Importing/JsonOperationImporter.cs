using System.Text.Json;
using HSEBank.Logic.Domain.Entities;
using HSEBank.Logic.Domain.Interfaces;
using HSEBank.Logic.Domain.Interfaces.Facades;
using HSEBank.Logic.Domain.Interfaces.Factories;

namespace HSEBank.Logic.Application.Importing;

public class JsonOperationImporter : OperationImporterTemplate
{
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public JsonOperationImporter(
        IOperationFacade ops,
        IRepository<BankAccount> accounts,
        IRepository<Category> categories,
        ICategoryFactory categoryFactory)
        : base(ops, accounts, categories, categoryFactory) { }

    public override string Id => "json";

    protected override IEnumerable<OperationImportRow> Parse(string content)
    {
        var rows = JsonSerializer.Deserialize<List<OperationImportRow>>(content, _jsonOptions);
        return rows ?? Enumerable.Empty<OperationImportRow>();
    }
}