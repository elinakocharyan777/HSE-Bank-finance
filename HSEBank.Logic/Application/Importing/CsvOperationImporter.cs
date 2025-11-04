using System.Globalization;
using HSEBank.Logic.Domain.Enums;
using HSEBank.Logic.Domain.Entities;
using HSEBank.Logic.Domain.Interfaces;
using HSEBank.Logic.Domain.Interfaces.Facades;
using HSEBank.Logic.Domain.Interfaces.Factories;

namespace HSEBank.Logic.Application.Importing;

public class CsvOperationImporter : OperationImporterTemplate
{
    public CsvOperationImporter(
        IOperationFacade ops,
        IRepository<BankAccount> accounts,
        IRepository<Category> categories,
        ICategoryFactory categoryFactory)
        : base(ops, accounts, categories, categoryFactory) { }

    public override string Id => "csv";

    protected override IEnumerable<OperationImportRow> Parse(string content)
    {
        // Ожидается первая строка-заголовок:
        // type,category,amount,date,description
        // date: ISO (yyyy-MM-dd) или локальный (парсим InvariantCulture)
        using var reader = new StringReader(content);
        var header = reader.ReadLine(); // пропустим
        string? line;
        while ((line = reader.ReadLine()) != null)
        {
            if (string.IsNullOrWhiteSpace(line)) continue;
            var parts = line.Split(',', StringSplitOptions.TrimEntries);

            if (parts.Length < 4) continue;

            var typeStr = parts[0];
            var category = parts[1];
            var amountStr = parts[2];
            var dateStr = parts[3];
            var desc = parts.Length > 4 ? parts[4] : null;

            if (!Enum.TryParse<FinanceType>(typeStr, true, out var type)) continue;
            if (!decimal.TryParse(amountStr, NumberStyles.Number, CultureInfo.InvariantCulture, out var amount)) continue;
            if (!DateTime.TryParse(dateStr, CultureInfo.InvariantCulture, DateTimeStyles.None, out var date)) continue;

            yield return new OperationImportRow
            {
                Type = type,
                Category = category,
                Amount = amount,
                Date = date,
                Description = desc
            };
        }
    }
}