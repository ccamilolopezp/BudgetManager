using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace BudgetManager.Module.BusinessObjects.EnterpriseAnalyzerObjects
{
    [DefaultClassOptions]
    [NavigationItem("Companies -")]
    [System.ComponentModel.DefaultProperty("Name")]
    public class Company : Institution
    {
        public Company() { }

        //[Browsable(false)]
        //public Int32 ID { get; protected set; }
        [Browsable(false)]
        public Int32 CompanyId { get; protected set; }

        public virtual CompanyTree CompanyTree { get; set; }
        public virtual ValueModel ValueModel { get; set; }
    }

    [DefaultClassOptions]
    public class AugmentedCompany : Company
    {
        public AugmentedCompany()
        {
            CompanyYearBudgets = new List<CompanyYearBudget>();
            CompanyYearAccountingResults = new List<CompanyYearAccountingResult>();
            //CompanyYearAccountingResultTs = new List<CompanyYearAccountingResultT>();
        }

        [Aggregated]
        public virtual List<CompanyYearBudget> CompanyYearBudgets { get; set; }

        [Aggregated]
        public virtual List<CompanyYearAccountingResult> CompanyYearAccountingResults { get; set; }

        //[Aggregated]
        //public virtual List<CompanyYearAccountingResultT> CompanyYearAccountingResultTs { get; set; }

        public virtual BudgetTree BudgetTree { get; set; }
        public virtual CompanyBudgetRules CompanyBudgetRules { get; set; }
        public virtual AccountingFrame AccountingFrame { get; set; }

        //public virtual CompanyBudgetRules CompanyBudgetRules { get; set; }
        //public virtual CompanyRules CompanyRules { get; set; }
    }

}
