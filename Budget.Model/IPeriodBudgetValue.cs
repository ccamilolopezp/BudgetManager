namespace Budget.Model
{
    public interface IPeriodBudgetValue : IPeriodValue
    {
        IBudgetAccount GetBudgetAccount();
        void UpdateExecutedAmount(bool ChangeEvents);
    }
}
