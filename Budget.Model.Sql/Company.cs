using Budget.XAF.Model.Sql;
using Finac.Business.Model;
using Hierarchy.Model;
using System.Collections.Generic;
using System.Linq;

namespace Budget.Model.Sql
{
    public class Company : IEntity<long>, ICompany
    {
        public virtual BudgetPeriodicity BudgetPeriodicity { get; set; }
        public virtual List<Tree> AccountingTrees { get; set; }
        public virtual List<BudgetUnit> Budgets { get; set; }
        public virtual List<ExecutedBudgetUnit> ExecutedBudgets { get; set; }

        public long Id { get; set; }

        public string Name { get; set; }
        public IList<ITree> GetAccountingTrees() => AccountingTrees.OfType<ITree>().ToList();
        public IList<IAccountingUnit<IBudgetAccount>> GetBudgetUnits() => Budgets.OfType<IAccountingUnit<IBudgetAccount>>().ToList();
        public IList<IAccountingUnit<IExecutedAccount>> GetExecutedBudgetUnits() => ExecutedBudgets.OfType<IAccountingUnit<IExecutedAccount>>().ToList();
        public IBudgetPeriodicity GetBudgetPeriodicity() => BudgetPeriodicity as IBudgetPeriodicity;

    }
}
