namespace HSEBank.Logic.Application.Exporting;

public interface IOperationExportVisitor
{
    void Begin();
    void Visit(OperationExportContext ctx);
    string End(); // отдать итоговую строку (CSV/JSON)
}