using Finac.Business.Model;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Budget.Model.Sql
{
    [DefaultProperty("Year")]
    public class ExecutedBudgetUnit : IEntity<long>, IAccountingUnit<IExecutedAccount>
    {
        public virtual Company Company { get; set; }
        public virtual List<ExecutedAccount> ExecutedAccounts { get; set; }

        public long Id { get; set; }

        public int Year { get; set; }
        public IList<IExecutedAccount> GetAccounts() => ExecutedAccounts.OfType<IExecutedAccount>().ToList();
        public ICompany GetCompany() => Company as ICompany;
    }
}
