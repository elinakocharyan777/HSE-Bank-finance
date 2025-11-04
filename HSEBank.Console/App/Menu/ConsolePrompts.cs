using System;
using System.Globalization;
using HSEBank.Logic.Domain.Enums;
using HSEBank.Logic.Domain.Interfaces.Facades;
using Microsoft.Extensions.DependencyInjection;
using HSEBank.Console.App;
namespace HSEBank.Console.App.Menu;

public class ConsolePrompts
{
    private readonly IServiceProvider _sp;

    public ConsolePrompts(IServiceProvider sp) => _sp = sp;

    public string Ask(string prompt, bool allowEmpty = false)
    {
        System.Console.Write(prompt);
        while (true)
        {
            var s = System.Console.ReadLine();
            if (s != null && (allowEmpty || !string.IsNullOrWhiteSpace(s))) return s;
            System.Console.Write("Повторите ввод: ");
        }
    }

    public int AskInt(string prompt, int min, int max)
    {
        System.Console.Write(prompt);
        while (true)
        {
            var s = System.Console.ReadLine();
            if (int.TryParse(s, out var v) && v >= min && v <= max) return v;
            System.Console.Write($"Число от {min} до {max}: ");
        }
    }

    public decimal AskDecimal(string prompt)
    {
        System.Console.Write(prompt);
        while (true)
        {
            var s = System.Console.ReadLine();
            if (decimal.TryParse(s, NumberStyles.Number, CultureInfo.InvariantCulture, out var v) && v >= 0)
                return v;
            System.Console.Write("Введите число >= 0: ");
        }
    }

    public DateTime AskDate(string prompt)
    {
        System.Console.Write(prompt);
        while (true)
        {
            var s = System.Console.ReadLine();
            if (DateTime.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt)) return dt;
            System.Console.Write("Формат yyyy-mm-dd: ");
        }
    }
    public DateTime AskDateNotFuture(string prompt)
    {
        System.Console.Write(prompt);
        while (true)
        {
            var s = System.Console.ReadLine();
            if (DateTime.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt))
            {
                if (dt <= DateTime.Today)
                    return dt;

                System.Console.Write("Дата не может быть в будущем. Повторите (yyyy-MM-dd): ");
                continue;
            }
            System.Console.Write("Формат yyyy-MM-dd: ");
        }
    }
    public FinanceType AskType(string prompt)
    {
        System.Console.Write(prompt);
        while (true)
        {
            var s = System.Console.ReadLine();
            if (Enum.TryParse<FinanceType>(s, true, out var t)) return t;
            System.Console.Write("Income или Expense: ");
        }
    }

    /// Выбор счёта. Если счетов нет, сообщаем и возвращаем false.
    public bool TrySelectAccount(out Guid accountId)
    {
        var accounts = _sp.GetRequiredService<IAccountFacade>().GetAll().ToList();
        if (accounts.Count == 0)
        {
            System.Console.WriteLine("Нет счетов. Сначала создайте счёт (пункт 1).");
            accountId = Guid.Empty;
            return false;
        }

        System.Console.WriteLine("Доступные счета:");
        for (int i = 0; i < accounts.Count; i++)
        {
            var a = accounts[i];
            System.Console.WriteLine($"{i + 1}) {a.Name} — {a.Balance} (id: {IdFormatter.Short(a.Id)})");
        }
        var idx = AskInt("Выбери номер: ", 1, accounts.Count) - 1;
        accountId = accounts[idx].Id;
        return true;
    }
    public bool TrySelectOperation(Guid accountId, out Guid operationId)
    {
        var opsFacade = _sp.GetRequiredService<IOperationFacade>();
        var list = opsFacade.GetByAccount(accountId).OrderBy(o => o.Date).ToList();
        if (list.Count == 0)
        {
            System.Console.WriteLine("Нет операций у выбранного счёта.");
            operationId = Guid.Empty;
            return false;
        }

        System.Console.WriteLine("Операции:");
        for (int i = 0; i < list.Count; i++)
        {
            var o = list[i];
            System.Console.WriteLine($"{i + 1}) {o.Date:yyyy-MM-dd} | {o.Type} | {o.Amount} | {IdFormatter.Short(o.Id)}");
        }

        var idx = AskInt("Выбери номер операции: ", 1, list.Count) - 1;
        operationId = list[idx].Id;
        return true;
    }
    public Guid SelectCategory(FinanceType type)
    {
        var catsFacade = _sp.GetRequiredService<ICategoryFacade>();
        var list = catsFacade.GetAll().Where(c => c.Type == type).ToList();
        if (list.Count == 0)
        {
            // Автосоздание дефолтной категории, если ничего нет
            return catsFacade.CreateCategory(type == FinanceType.Income ? "Прочие доходы" : "Прочие расходы", type);
        }

        System.Console.WriteLine("Доступные категории:");
        for (int i = 0; i < list.Count; i++)
            System.Console.WriteLine($"{i + 1}) {list[i].Name} ({list[i].Id})");
        var idx = AskInt("Выбери номер: ", 1, list.Count) - 1;
        return list[idx].Id;
    }
}