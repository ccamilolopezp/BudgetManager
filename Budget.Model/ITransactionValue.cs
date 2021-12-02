namespace Budget.Model
{
    public interface ITransactionValue
    {
        string Description { get; set; }
        string FullAccountCode { get; set; }
        TransactionType TransactionType { get; set; }
        double Amount { get; set; }
        string Project { get; set; }
    }

    public enum TransactionType
    {
        Quoted = 0,
        Debit = 1,
        Credit = -1,
        Executed = 2
    }
}
