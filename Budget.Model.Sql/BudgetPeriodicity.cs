using Finac.Business.Model;
using System.Collections.Generic;
using System.Linq;

namespace Budget.Model.Sql
{
    public class BudgetPeriodicity : IEntity<long>, IBudgetPeriodicity
    {
        public virtual List<BudgetPeriod> BudgetPeriods { get; set; }

        public long Id { get; set; }

        public string Name { get; set; }
        public int PeriodInMonths { get; set; }
        public IList<IBudgetPeriod> GetBudgetPeriods() => BudgetPeriods.OfType<IBudgetPeriod>().ToList();
    }
}
