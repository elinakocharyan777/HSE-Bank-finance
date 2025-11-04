using System.Text.Json;

namespace HSEBank.Logic.Application.Exporting;

public class JsonOperationVisitor : IOperationExportVisitor
{
    private readonly List<OperationExportContext> _items = new();

    public void Begin() => _items.Clear();

    public void Visit(OperationExportContext ctx) => _items.Add(ctx);

    public string End() => JsonSerializer.Serialize(_items, new JsonSerializerOptions
    {
        WriteIndented = true
    });
}