using Finac.Business.Model;

namespace Budget.Model.Sql
{
    public class TransactionExecutedValue : IEntity<long>, ITransactionExecutedValue
    {
        public virtual PeriodExecutedValue PeriodExecutedValue { get; set; }

        public long Id { get; set; }

        public string Description { get; set; }
        public string FullAccountCode { get; set; }
        public TransactionType TransactionType { get; set; }
        public double Amount { get; set; }
        public string Project { get; set; }
        public IPeriodExecutedValue GetPeriodExecutedValue() => PeriodExecutedValue as IPeriodExecutedValue;
    }
}
