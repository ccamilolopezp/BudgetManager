using Finac.Business.Model;
using Hierarchy.Model;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Budget.Model.Sql
{
    [DefaultProperty("Description")]
    public class ExecutedAccount : IEntity<long>, IExecutedAccount
    {
        public virtual ExecutedBudgetUnit ExecutedBudgetUnit { get; set; }
        public virtual Budget.XAF.Model.Sql.Node AccountingTreeNode { get; set; }
        public virtual LinkExecutedBudget LinkExecutedBudget { get; set; }
        public virtual List<PeriodExecutedValue> PeriodExecutedValues { get; set; }

        public long Id { get; set; }

        public string Description { get; set; }
        public double YearAmount { get; private set; }

        public INode GetAccountingTreeNode() => AccountingTreeNode as INode;
        public IAccountingUnit<IExecutedAccount> GetAccountingUnit() => ExecutedBudgetUnit as IAccountingUnit<IExecutedAccount>;
        public IList<IPeriodValue> GetPeriodValues() => PeriodExecutedValues.OfType<IPeriodValue>().ToList();

        public void UpdateAccountingResultByComponentStatus(bool ChangeEvents)
        {
            YearAmount = PeriodExecutedValues.Select(periodBudgetValue => periodBudgetValue.PeriodAmount).Sum();
            if (ChangeEvents) YearAmount = YearAmount;
        }
    }
}
