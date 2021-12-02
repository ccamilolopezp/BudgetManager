using Finac.Business.Model;
using Hierarchy.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Budget.Model.Sql
{
    [DefaultProperty("Description")]
    public class BudgetAccount : IEntity<long>, IBudgetAccount
    {
        public virtual BudgetUnit BudgetUnit { get; set; }
        public virtual Budget.XAF.Model.Sql.Node AccountingTreeNode { get; set; }
        public virtual LinkExecutedBudget LinkExecutedBudget { get; set; }
        public virtual List<PeriodBudgetValue> PeriodBudgetValues { get; set; }

        public long Id { get; set; }

        public string Description { get; set; }
        public double YearAmount { get; private set; }
        public double YearExecutedBudget { get; private set; }
        public double YearRemainingBudget { get; private set; }
        public INode GetAccountingTreeNode() => AccountingTreeNode as INode;
        public IAccountingUnit<IBudgetAccount> GetAccountingUnit() => BudgetUnit as IAccountingUnit<IBudgetAccount>;
        public IList<IPeriodValue> GetPeriodValues() => PeriodBudgetValues.OfType<IPeriodValue>().ToList();

        public void UpdateComponentBudgetStatus(bool ChangeEvents)
        {
            if (PeriodBudgetValues == null) return;
            if (PeriodBudgetValues.Count() == 0) return;
            var valid = PeriodBudgetValues.Where(periodBudgetValue => periodBudgetValue.PeriodDate != DateTime.MinValue);
            if (valid == null) return;
            if (valid.Count() == 0) return;

            var amount = valid.Sum(periodBudgetValue => periodBudgetValue.PeriodAmount);
            var executedBudget = valid.Sum(periodBudgetValue => periodBudgetValue.PeriodExecutedBudget);
            var remainingBudget = amount - Math.Abs(executedBudget);
            if (ChangeEvents)
            {
                YearAmount = amount;
                YearExecutedBudget = executedBudget;
                YearRemainingBudget = remainingBudget;
            }
        }
    }
}
