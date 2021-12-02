namespace Budget.Model
{
    public interface IExecutedAccount : IAccount<IExecutedAccount>
    {
        void UpdateAccountingResultByComponentStatus(bool ChangeEvents);
    }
}
