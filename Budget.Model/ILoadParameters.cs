namespace Budget.Model
{
    public interface ILoadParameters
    {
        string Name { get; set; }
        string CultureInfo { get; set; }

        bool IncludeAccount { get; set; }
        int AccountColumnStart { get; set; }
        int LastColumnWithAccount { get; set; }

        bool IncludeAccountName { get; set; }
        int AccountNameColumn { get; set; }

        bool IncludeProcessDate { get; set; }
        int ProcessDateColumn { get; set; }

        bool IncludeNit { get; set; }
        int NitColumn { get; set; }

        bool IncludeValues { get; set; }
        int ValueColumnStart { get; set; }
        int LastColumnWithValue { get; set; }

        bool IncludeDebitTransaction { get; set; }
        int DebitTransactionColumn { get; set; }

        bool IncludeCreditTransaction { get; set; }
        int CreditTransactionColumn { get; set; }

        bool ForceCreateMissingNodes { get; set; }

        int TotalAccountColumns { get; }
        int TotalValueColumns { get; }
        int MaxColumns();

        Hierarchy.Model.ITreeName GetTreeName();
    }
}
