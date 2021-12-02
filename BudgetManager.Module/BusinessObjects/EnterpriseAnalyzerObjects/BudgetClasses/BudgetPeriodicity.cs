using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace BudgetManager.Module.BusinessObjects.EnterpriseAnalyzerObjects
{
    [DefaultClassOptions]
    [NavigationItem("Z- Budgets -")]
    [System.ComponentModel.DefaultProperty("Name")]
    public class BudgetPeriodicity : FinacBaseObject
    {
        [Browsable(false)]
        public Int32 ID { get; protected set; }

        public BudgetPeriodicity()
        {
            BudgetPeriods = new List<BudgetPeriod>();
        }

        [Aggregated]
        public virtual List<BudgetPeriod> BudgetPeriods { get; set; }

        public string Name { get; set; }

        public int PeriodInMonths { get; set; }
    }
}
