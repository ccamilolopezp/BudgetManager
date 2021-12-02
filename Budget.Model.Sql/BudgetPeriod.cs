using Finac.Business.Model;

namespace Budget.Model.Sql
{
    public class BudgetPeriod : IEntity<long>, IBudgetPeriod
    {
        public virtual BudgetPeriodicity BudgetPeriodicity { get; set; }

        public long Id { get; set; }

        public int Consecutive { get; set; }
        public string Name { get; set; }
        public int InitialMonth { get; set; }
        public int FinalMonth { get; set; }
        public IBudgetPeriodicity GetBudgetPeriodicity() => BudgetPeriodicity as IBudgetPeriodicity;
    }
}
