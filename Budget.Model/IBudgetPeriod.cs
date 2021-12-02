namespace Budget.Model
{
    public interface IBudgetPeriod
    {
        IBudgetPeriodicity GetBudgetPeriodicity();
        int Consecutive { get; set; }
        string Name { get; set; }
        int InitialMonth { get; set; }
        int FinalMonth { get; set; }
    }
}
