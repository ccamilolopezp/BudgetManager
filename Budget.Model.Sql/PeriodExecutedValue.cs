using Finac.Business.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Budget.Model.Sql
{
    public class PeriodExecutedValue : IEntity<long>, IPeriodExecutedValue
    {
        public virtual ExecutedAccount ExecutedAccount { get; set; }
        public virtual List<TransactionExecutedValue> TransactionValues { get; set; }
        public double OpeningBalance { get; set; }
        public double DebitAmount { get; private set; }
        public double CreditAmount { get; private set; }
        public double ClosingBalance { get; set; }
        public double CalculatedClosingBalance { get; private set; }

        public long Id { get; set; }

        public double PeriodAmount { get; private set; }
        public DateTime PeriodDate { get; set; }
        public string Description { get; set; }
        public DateTime EndOfPeriod { get; set; }
        public string AccountCode { get; set; }

        public IExecutedAccount GetExecutedAccount() => ExecutedAccount as IExecutedAccount;
        public IList<ITransactionValue> GetTransactions() => TransactionValues.OfType<ITransactionValue>().ToList();

        public void UpdatePeriodAmount(bool ChangeEvents)
        {
            if (!ChangeEvents) return;
            if (TransactionValues == null) return;
            if (TransactionValues.Count() == 0) return;

            var transactionSummaries = TransactionValues.GroupBy(transaction => transaction.TransactionType)
                                                        .ToDictionary(transactionByType => transactionByType.Key,
                                                                      transactionByType => transactionByType.ToList()
                                                                                                            .Sum(transaction => transaction.Amount));

            if (transactionSummaries.ContainsKey(TransactionType.Credit)) CreditAmount = transactionSummaries[TransactionType.Credit];
            if (transactionSummaries.ContainsKey(TransactionType.Debit)) DebitAmount = transactionSummaries[TransactionType.Debit];

            PeriodAmount = DebitAmount - CreditAmount;
            CalculatedClosingBalance = OpeningBalance + PeriodAmount;

        }
    }
}
