namespace HSEBank.Logic.Application.Analytics;

public interface IAnalyticsService
{
    // Сводка по типам (доход/расход/итог) за период
    TypeTotals GetTypeTotals(Guid accountId, DateTime from, DateTime to);

    // Топ категорий за период (суммы со знаком +/− в зависимости от типа)
    IReadOnlyList<CategoryTotal> GetTotalsByCategory(Guid accountId, DateTime from, DateTime to);
}