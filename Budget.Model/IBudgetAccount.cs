namespace Budget.Model
{
    public interface IBudgetAccount : IAccount<IBudgetAccount>
    {
        double YearExecutedBudget { get; }
        double YearRemainingBudget { get; }
        void UpdateComponentBudgetStatus(bool ChangeEvents);
    }
}
