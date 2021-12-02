namespace Budget.Model
{
    public interface ITransactionExecutedValue : ITransactionValue
    {
        IPeriodExecutedValue GetPeriodExecutedValue();
    }
}
