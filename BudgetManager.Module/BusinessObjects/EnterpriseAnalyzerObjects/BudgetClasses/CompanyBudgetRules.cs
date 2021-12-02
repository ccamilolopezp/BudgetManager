using DevExpress.Persistent.Base;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace BudgetManager.Module.BusinessObjects.EnterpriseAnalyzerObjects
{
    [NavigationItem("Z-Budgets -")]
    [System.ComponentModel.DefaultProperty("Company.Name")]
    [DefaultClassOptions]
    public class CompanyBudgetRules : BudgetBaseObject
    {
        [Browsable(false)]
        [ForeignKey("Company")]
        public Int32 CompanyBudgetRulesId { get; protected set; }
        public virtual AugmentedCompany Company { get; set; }

        public CompanyBudgetRules() { }

        public int InitialDay { get; set; }

        public int InitialMonth { get; set; }

        public virtual BudgetPeriodicity BudgetPeriodicity { get; set; }
    }
}
