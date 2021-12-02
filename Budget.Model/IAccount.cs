using Hierarchy.Model;
using System.Collections.Generic;

namespace Budget.Model
{
    public interface IAccount<T>
    {
        IAccountingUnit<T> GetAccountingUnit();
        INode GetAccountingTreeNode();
        IList<IPeriodValue> GetPeriodValues();
        string Description { get; set; }
        double YearAmount { get; }

    }
}
