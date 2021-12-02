using Hierarchy.Model;
using System.Collections.Generic;

namespace Budget.Model
{
    public interface ICompany
    {
        IBudgetPeriodicity GetBudgetPeriodicity();
        IList<ITree> GetAccountingTrees();
        IList<IAccountingUnit<IBudgetAccount>> GetBudgetUnits();
        IList<IAccountingUnit<IExecutedAccount>> GetExecutedBudgetUnits();
        string Name { get; set; }
    }
}
