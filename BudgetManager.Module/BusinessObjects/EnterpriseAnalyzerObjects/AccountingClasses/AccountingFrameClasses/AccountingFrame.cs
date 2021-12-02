using DevExpress.Persistent.Base;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace BudgetManager.Module.BusinessObjects.EnterpriseAnalyzerObjects
{
    [NavigationItem("Z-Accounting")]
    [DefaultClassOptions]
    [System.ComponentModel.DefaultProperty("Company.Name")]
    public class AccountingFrame : CompanyDataObject
    {
        [Browsable(false)]
        [ForeignKey("Company")]
        public Int32 AccountingFrameId { get; protected set; }
        public virtual AugmentedCompany Company { get; set; }

        public virtual ValueModelTree AccountingPlanTree { get; set; }

        ////Cambio a ValueModelTree
        ////public AccountingPlanTree()
        ////{
        ////    AccountingPlanTreeNodes = new List<AccountingPlanTreeNode>();
        ////}

        ////[Aggregated]
        ////public virtual IList<AccountingPlanTreeNode> AccountingPlanTreeNodes { get; set; }
    }
}
