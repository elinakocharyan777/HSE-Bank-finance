using System.Globalization;
using System.Text;

namespace HSEBank.Logic.Application.Exporting;

public class CsvOperationVisitor : IOperationExportVisitor
{
    private readonly StringBuilder _sb = new();

    public void Begin()
    {
        _sb.Clear();
        _sb.AppendLine("id,type,category,amount,date,description");
    }

    public void Visit(OperationExportContext ctx)
    {
        // простейшее экранирование: заменить запятую в описании
        var desc = ctx.Description?.Replace(",", " ") ?? "";
        _sb.AppendLine(string.Join(",",
            ctx.OperationId,
            ctx.Type,
            ctx.CategoryName,
            ctx.Amount.ToString(CultureInfo.InvariantCulture),
            ctx.Date.ToString("yyyy-MM-dd"),
            desc));
    }

    public string End() => _sb.ToString();
}