using System.Collections.Generic;

namespace Budget.Model
{
    public interface IBudgetPeriodicity
    {
        IList<IBudgetPeriod> GetBudgetPeriods();
        string Name { get; set; }
        int PeriodInMonths { get; set; }
    }
}
