namespace Budget.Model
{
    public interface IPeriodExecutedValue : IPeriodValue
    {
        IExecutedAccount GetExecutedAccount();
    }
}
