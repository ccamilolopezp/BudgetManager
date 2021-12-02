using Finac.Business.Model;
using Hierarchy.Model;

namespace Budget.Model.Sql
{
    public class LoadParameters : IEntity<long>, ILoadParameters
    {
        public Budget.XAF.Model.Sql.TreeName TreeName { get; set; }

        public long Id { get; set; }

        public string Name { get; set; }

        public string CultureInfo { get; set; }

        public bool IncludeAccount { get; set; }
        public int AccountColumnStart { get; set; }
        public int LastColumnWithAccount { get; set; }

        public bool IncludeAccountName { get; set; }
        public int AccountNameColumn { get; set; }

        public bool IncludeProcessDate { get; set; }
        public int ProcessDateColumn { get; set; }

        public bool IncludeNit { get; set; }
        public int NitColumn { get; set; }

        public bool IncludeValues { get; set; }
        public int ValueColumnStart { get; set; }
        public int LastColumnWithValue { get; set; }

        public bool IncludeDebitTransaction { get; set; }
        public int DebitTransactionColumn { get; set; }

        public bool IncludeCreditTransaction { get; set; }
        public int CreditTransactionColumn { get; set; }

        public bool ForceCreateMissingNodes { get; set; }

        public int TotalAccountColumns { get { return LastColumnWithAccount - AccountColumnStart + 1; } }

        public int TotalValueColumns { get { return LastColumnWithValue - ValueColumnStart + 1; } }

        public int MaxColumns()
        {
            int totalColumns = 0;
            if (IncludeAccount) totalColumns = totalColumns + TotalAccountColumns;
            if (IncludeValues) totalColumns = totalColumns + TotalValueColumns;
            if (IncludeAccountName) totalColumns++;
            if (IncludeProcessDate) totalColumns++;
            if (IncludeNit) totalColumns++;            
            return totalColumns;
        }

        public ITreeName GetTreeName() => TreeName as ITreeName;
    }
}
