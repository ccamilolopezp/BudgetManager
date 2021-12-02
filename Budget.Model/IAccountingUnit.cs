using Budget.Model;
using System.Collections.Generic;

namespace Budget.Model
{
    public interface IAccountingUnit<T>
    {
        ICompany GetCompany();
        IList<T> GetAccounts();
        int Year { get; set; }
    }
}
