namespace HSEBank.Logic.Application.Importing.Validation;

public abstract class ImportRuleBase : IImportRule
{
    private IImportRule? _next;

    public IImportRule SetNext(IImportRule next)
    {
        _next = next;
        return next;
    }

    public bool Validate(OperationImportRow row)
    {
        if (!Check(row)) return false;
        return _next?.Validate(row) ?? true;
    }

    protected abstract bool Check(OperationImportRow row);
}