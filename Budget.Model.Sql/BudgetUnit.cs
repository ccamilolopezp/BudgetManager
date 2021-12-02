using Finac.Business.Model;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Budget.Model.Sql
{
    [DefaultProperty("Year")]
    public class BudgetUnit : IEntity<long>, IAccountingUnit<IBudgetAccount>
    {
        public virtual Company Company { get; set; }
        public virtual List<BudgetAccount> BudgetAccounts { get; set; }

        public long Id { get; set; }

        public int Year { get; set; }
        public IList<IBudgetAccount> GetAccounts() => BudgetAccounts.OfType<IBudgetAccount>().ToList();
        public ICompany GetCompany() => Company as ICompany;      
    }
}
