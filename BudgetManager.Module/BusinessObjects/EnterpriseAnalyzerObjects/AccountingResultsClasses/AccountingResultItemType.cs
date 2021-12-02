using DevExpress.Persistent.Base;
using System;
using System.ComponentModel;

namespace BudgetManager.Module.BusinessObjects.EnterpriseAnalyzerObjects
{
    [NavigationItem("Z-Accounting")]
    [System.ComponentModel.DefaultProperty("Description")]
    [DefaultClassOptions]
    public class AccountingResultItemType
    {
        [Browsable(false)]
        public Int32 AccountingResultItemTypeId { get; protected set; }

        public AccountingResultItemType() { }

        public string Name { get; set; }

        public bool IsDebit { get; set; }

        public bool IsCredit { get; set; }

        public bool IsOpeningBalance { get; set; }

        public bool IsClosingBalance { get; set; }
    }
}
