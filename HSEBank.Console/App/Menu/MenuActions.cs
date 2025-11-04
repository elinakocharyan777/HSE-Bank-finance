using System;
using System.IO;
using System.Linq;
using System.Globalization;
using Microsoft.Extensions.DependencyInjection;
using HSEBank.Logic.Application.Commands;
using HSEBank.Logic.Application.Decorators;
using HSEBank.Logic.Application.Exporting;
using HSEBank.Logic.Application.Importing;
using HSEBank.Logic.Application.Interfaces;
using HSEBank.Logic.Application.Analytics;
using HSEBank.Logic.Domain.Enums;
using HSEBank.Logic.Domain.Interfaces.Facades;
using HSEBank.Console.App;

namespace HSEBank.Console.App.Menu;

public class MenuActions
{
    private readonly IServiceProvider _sp;
    private readonly ConsolePrompts _ask;

    public MenuActions(IServiceProvider sp, ConsolePrompts prompts)
    {
        _sp = sp;
        _ask = prompts;
    }

    public void CreateAccount()
    {
        var accounts = _sp.GetRequiredService<IAccountFacade>();
        var name = _ask.Ask("Название счёта: ");
        var init = _ask.AskDecimal("Начальный баланс (число): ");
        var id = new TimedCommand<Guid>(
            new CreateAccountCommand(accounts, name, init),
            d => System.Console.WriteLine($"CreateAccount выполняется за: {d.TotalMilliseconds:F1} миллисекунд")
        ).Execute();
        System.Console.WriteLine($"Создан счёт: {IdFormatter.Short(id)}");
    }

    public void CreateCategory()
    {
        var categories = _sp.GetRequiredService<ICategoryFacade>();
        var name = _ask.Ask("Название категории: ");
        var type = _ask.AskType("Тип (Income/Expense): ");
        var id = new CreateCategoryCommand(categories, name, type).Execute();
        System.Console.WriteLine($"Создана категория: {id}");
    }

    public void AddOperation()
    {
        if (!_ask.TrySelectAccount(out var accountId)) return;
        var ops = _sp.GetRequiredService<IOperationFacade>();
        var type = _ask.AskType("Тип операции (Income/Expense): ");
        var categoryId = _ask.SelectCategory(type);
        var amount = _ask.AskDecimal("Сумма: ");
        var date = _ask.AskDateNotFuture("Дата (yyyy-MM-dd, не в будущем): ");
        var desc = _ask.Ask("Описание (опционально): ", allowEmpty: true);
        var accountsFacade = _sp.GetRequiredService<IAccountFacade>();
        var account = accountsFacade.GetById(accountId)!;

        if (type == FinanceType.Expense && account.Balance < amount)
        {
            System.Console.WriteLine(
                $"Недостаточно средств. Доступно: {account.Balance}, требуется: {amount}.");
            return; // не добавляем операцию
        }
        var id = new TimedCommand<Guid>(
            new AddOperationCommand(ops, type, accountId, categoryId, amount, date, desc),
            d => System.Console.WriteLine($"AddOperation выполняется за: {d.TotalMilliseconds:F1} миллисекунд")
        ).Execute();

        System.Console.WriteLine($"Операция добавлена: {IdFormatter.Short(id)}");
    }
    public void EditOperation()
    {
        if (!_ask.TrySelectAccount(out var accountId)) return;
        if (!_ask.TrySelectOperation(accountId, out var operationId)) return;

        var opsFacade = _sp.GetRequiredService<IOperationFacade>();
        var accountsFacade = _sp.GetRequiredService<IAccountFacade>();
        var account = accountsFacade.GetById(accountId)!;

        // Возьмём текущую операцию, чтобы посчитать дельту (для проверки «недостаточно средств»)
        var current = opsFacade.GetByAccount(accountId).First(o => o.Id == operationId);

        var newType = _ask.AskType($"Новый тип (Income/Expense), текущее: {current.Type}: ");
        var newCategoryId = _ask.SelectCategory(newType);
        var newAmount = _ask.AskDecimal($"Новая сумма (текущая: {current.Amount}): ");
        var newDate = _ask.AskDateNotFuture($"Новая дата (текущая: {current.Date:yyyy-MM-dd}): ");
        var newDesc = _ask.Ask("Новое описание (опционально): ", allowEmpty: true);

        // Проверка баланса: не уходим в минус
        var currentEffect = current.Type == FinanceType.Income ? current.Amount : -current.Amount;
        var newEffect = newType == FinanceType.Income ? newAmount : -newAmount;
        var delta = newEffect - currentEffect;
        if (account.Balance + delta < 0)
        {
            System.Console.WriteLine($"Недостаточно средств для изменения. Доступно: {account.Balance}, требуется ещё: {-(account.Balance + delta)}.");
            return;
        }

        var ok = opsFacade.UpdateOperation(operationId, newType, newCategoryId, newAmount, newDate, newDesc);
        System.Console.WriteLine(ok ? "Операция обновлена." : "Операция не найдена.");
    }
    public void ImportOps()
    {
        if (!_ask.TrySelectAccount(out var accountId)) return;
        var path = _ask.Ask("Путь к файлу (CSV/JSON): ");
        var importer = _sp.GetRequiredService<IOperationImporterFactory>().ResolveByExtension(path);
        var count = importer.Import(path, accountId);
        System.Console.WriteLine($"Импортировано: {count}");
    }

    public void ExportOps()
    {
        if (!_ask.TrySelectAccount(out var accountId)) return;
        var exporter = _sp.GetRequiredService<OperationExporter>();
        var format = _ask.Ask("Формат (csv/json): ").ToLowerInvariant();
        switch (format)
        {
            case "csv":
                var csvVis = _sp.GetServices<IOperationExportVisitor>().OfType<CsvOperationVisitor>().First();
                var csv = exporter.ExportForAccount(accountId, csvVis);
                File.WriteAllText("export-ops.csv", csv);
                System.Console.WriteLine("CSV экспортирован: export-ops.csv");
                break;
            case "json":
                var jsonVis = _sp.GetServices<IOperationExportVisitor>().OfType<JsonOperationVisitor>().First();
                var json = exporter.ExportForAccount(accountId, jsonVis);
                File.WriteAllText("export-ops.json", json);
                System.Console.WriteLine("JSON экспортирован: export-ops.json");
                break;
            default:
                System.Console.WriteLine("Неизвестный формат.");
                break;
        }
    }

    public void ShowAnalytics()
    {
        if (!_ask.TrySelectAccount(out var accountId)) return;
        var analytics = _sp.GetRequiredService<IAnalyticsService>();
        var from = _ask.AskDate("Начало периода (yyyy-mm-dd): ");
        var to = _ask.AskDate("Конец периода (yyyy-mm-dd): ");

        var totals = analytics.GetTypeTotals(accountId, from, to);
        System.Console.WriteLine($"Доход:  {totals.Income}");
        System.Console.WriteLine($"Расход: {totals.Expense}");
        System.Console.WriteLine($"Итог:   {totals.Net}");

        var byCat = analytics.GetTotalsByCategory(accountId, from, to);
        System.Console.WriteLine("\nПо категориям:");
        foreach (var c in byCat)
            System.Console.WriteLine($" - {c.CategoryName}: {c.Total}");
    }

    public void RecalcBalance()
    {
        if (!_ask.TrySelectAccount(out var accountId)) return;
        var cmd = new TimedCommand<bool>(
            new RecalculateBalanceCommand(_sp.GetRequiredService<IBalanceService>(), accountId),
            d => System.Console.WriteLine($"RecalcBalance выполняется за: {d.TotalMilliseconds:F1} миллисекунд"));
        cmd.Execute();
        System.Console.WriteLine("Баланс пересчитан.");
    }

    public void ShowAccounts()
    {
        var accounts = _sp.GetRequiredService<IAccountFacade>();
        foreach (var a in accounts.GetAll())
            System.Console.WriteLine($"{IdFormatter.Short(a.Id)} | {a.Name} | Баланс: {a.Balance}");
    }
}