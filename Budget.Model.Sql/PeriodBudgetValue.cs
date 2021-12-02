using Finac.Business.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Budget.Model.Sql
{
    public class PeriodBudgetValue : IEntity<long>, IPeriodBudgetValue
    {
        public virtual BudgetAccount BudgetAccount { get; set; }
        public virtual List<TransactionBudgetValue> TransactionValues { get; set; }
        public double PeriodExecutedBudget { get; private set; }
        public double PeriodRemainingBudget { get { return PeriodAmount - Math.Abs(PeriodExecutedBudget); } }

        public long Id { get; set; }

        public double PeriodAmount { get; private set; }
        public DateTime PeriodDate { get; set; }
        public string Description { get; set; }
        public DateTime EndOfPeriod { get; set; }
        public string AccountCode { get; set; }
        public IBudgetAccount GetBudgetAccount() => BudgetAccount as IBudgetAccount;
        public IList<ITransactionValue> GetTransactions() => TransactionValues.OfType<ITransactionValue>().ToList();

        public void UpdateExecutedAmount(bool ChangeEvents)
        {
            if (BudgetAccount.LinkExecutedBudget == null) return;
            if (BudgetAccount.LinkExecutedBudget.ExecutedAccountId == null)
            {
                if(ChangeEvents)PeriodExecutedBudget = 0;
                return;
            }

            var executedBudget = BudgetAccount.LinkExecutedBudget.ExecutedAccountId.PeriodExecutedValues.Where(p => p.PeriodDate.Month == PeriodDate.Month && p.PeriodDate.Year == PeriodDate.Year)
                                                                                                        .Sum(a => a.PeriodAmount);

            if (ChangeEvents)
            {
                PeriodExecutedBudget = executedBudget;
            }
        }

        public void UpdatePeriodAmount(bool ChangeEvents)
        {
            if (TransactionValues == null) return;
            if (TransactionValues.Count() == 0) return;

            var periodAmount = TransactionValues.Sum(transactionValue => transactionValue.Amount);

            if (ChangeEvents)
            {
                PeriodAmount = periodAmount;
            }
        }
    }
}
