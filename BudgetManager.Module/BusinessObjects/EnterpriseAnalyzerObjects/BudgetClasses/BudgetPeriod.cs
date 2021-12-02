using DevExpress.Persistent.Base;
using System;
using System.ComponentModel;

namespace BudgetManager.Module.BusinessObjects.EnterpriseAnalyzerObjects
{
    [DefaultClassOptions]
    [NavigationItem("Z- Budgets -")]
    [System.ComponentModel.DefaultProperty("Name")]
    public class BudgetPeriod : FinacBaseObject
    {
        [Browsable(false)]
        public Int32 ID { get; protected set; }

        private BudgetPeriodicity fBudgetPeriodicity;
        public virtual BudgetPeriodicity BudgetPeriodicity
        {
            get { return fBudgetPeriodicity; }
            set { fBudgetPeriodicity = value; }
        }

        public int Consecutive { get; set; }

        public string Name { get; set; }

        public int InitialMonth { get; set; }

        public int FinalMonth { get; set; }
    }
}
