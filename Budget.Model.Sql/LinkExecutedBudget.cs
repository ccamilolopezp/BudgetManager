using Finac.Business.Model;
using System.ComponentModel;

namespace Budget.Model.Sql
{
    [DefaultProperty("ExecutedAccountId")]
    public class LinkExecutedBudget : IEntity<long>
    {
        public virtual BudgetAccount BudgetAccountId { get; set; }
        public virtual ExecutedAccount ExecutedAccountId { get; set; }

        public long Id { get; set; }

        public double PeriodAmount => ExecutedAccountId == null ? 0.0 : ExecutedAccountId.YearAmount;
    }
}
