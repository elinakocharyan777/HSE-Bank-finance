namespace HSEBank.Logic.Application.Importing.Validation;

public class DateNotFutureRule : ImportRuleBase
{
    protected override bool Check(OperationImportRow row) =>
        row.Date <= DateTime.Today.AddDays(1); // допустим "сегодня/завтра" как ок
}