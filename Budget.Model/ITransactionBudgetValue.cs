namespace Budget.Model
{
    public interface ITransactionBudgetValue : ITransactionValue
    {
        IPeriodBudgetValue GetPeriodBudgetValue();
    }
}
