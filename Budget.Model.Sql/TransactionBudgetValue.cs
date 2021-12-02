using Finac.Business.Model;

namespace Budget.Model.Sql
{
    public class TransactionBudgetValue : IEntity<long>, ITransactionBudgetValue
    {
        public virtual PeriodBudgetValue PeriodBudgetValue { get; set; }

        public long Id { get; set; }

        public string Description { get; set; }
        public string FullAccountCode { get; set; }
        public TransactionType TransactionType { get; set; }
        public double Amount { get; set; }
        public string Project { get; set; }
        public IPeriodBudgetValue GetPeriodBudgetValue() => PeriodBudgetValue as IPeriodBudgetValue;
    }
}
