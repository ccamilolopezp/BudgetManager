using System;
using System.Collections.Generic;

namespace Budget.Model
{
    public interface IPeriodValue
    {
        IList<ITransactionValue> GetTransactions();
        double PeriodAmount { get; }
        DateTime PeriodDate { get; set; }
        string Description { get; set; }
        DateTime EndOfPeriod { get; set; }
        string AccountCode { get; set; }
        void UpdatePeriodAmount(bool ChangeEvents);
    }
}
