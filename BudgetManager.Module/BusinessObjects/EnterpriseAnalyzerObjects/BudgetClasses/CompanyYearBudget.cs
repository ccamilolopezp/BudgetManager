using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace BudgetManager.Module.BusinessObjects.EnterpriseAnalyzerObjects
{
    [NavigationItem("Budgets -")]
    [System.ComponentModel.DefaultProperty("CompanyYear")]
    //[System.ComponentModel.DefaultProperty("Company")]
    [DefaultClassOptions]

    public class CompanyYearBudget : BudgetBaseObject
    {
        [Browsable(false)]
        public Int32 CompanyYearBudgetId { get; protected set; }

        private AugmentedCompany fCompany;

        public virtual AugmentedCompany Company
        {
            get { return fCompany; }
            set { fCompany = value; }
        }

        public CompanyYearBudget()
        {
            ComponentBudgets = new List<ComponentBudget>();
        }

        [Aggregated]
        public virtual List<ComponentBudget> ComponentBudgets { get; set; }

        public int Year { get; set; }
        public bool IncludesProductBudget { get; set; }

        [NotMapped]
        public string CompanyYear
        {
            get { return Company == null ? Year.ToString() : Company.Name + " - " + Year.ToString(); }
        }

        private int fMaximumPeriodWithExecutionValues { get; set; }
        public int MaximumPeriodWithExecutionValues
        {
            get { return fMaximumPeriodWithExecutionValues; }
            protected set { fMaximumPeriodWithExecutionValues = value; }
        }

        public void UpdateCompanyYearBudgetStatus(bool forceChangeEvents)
        {
            int oldMaximumPeriodWithExecutionValues = fMaximumPeriodWithExecutionValues;

            int maximumPeriodWithExecutionValues = 0;

            foreach (ComponentBudget componentBudget in ComponentBudgets)
            {
                if (componentBudget.MaximumPeriodWithExecutionValues > 0)
                {
                    maximumPeriodWithExecutionValues =
                        Math.Max(maximumPeriodWithExecutionValues, componentBudget.MaximumPeriodWithExecutionValues);
                }
            }

            fMaximumPeriodWithExecutionValues = maximumPeriodWithExecutionValues;

            if (forceChangeEvents)
            {
                MaximumPeriodWithExecutionValues = fMaximumPeriodWithExecutionValues;
            }
        }
    }
}
