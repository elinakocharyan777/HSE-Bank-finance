namespace HSEBank.Logic.Application.Importing.Validation;

public static class ImportRulesFactory
{
    public static IImportRule BuildDefault()
    {
        var root = new AmountPositiveRule();
        root.SetNext(new CategoryNotEmptyRule())
            .SetNext(new DateNotFutureRule());
        return root;
    }
}