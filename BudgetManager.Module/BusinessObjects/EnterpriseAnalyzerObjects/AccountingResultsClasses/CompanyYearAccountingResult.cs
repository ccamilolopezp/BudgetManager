using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace BudgetManager.Module.BusinessObjects.EnterpriseAnalyzerObjects
{
    [NavigationItem("Accounting Results")]
    //[System.ComponentModel.DefaultProperty("'Company.Name'+'-'+(Year.ToString())")]
    [System.ComponentModel.DefaultProperty("Company")]
    [DefaultClassOptions]
    public class CompanyYearAccountingResult : CompanyDataObject
    {
        [Browsable(false)]
        public Int32 CompanyYearAccountingResultId { get; protected set; }

        public CompanyYearAccountingResult()
        {
            AccountingResultByComponents = new List<AccountingResultByComponent>();
        }

        private AugmentedCompany fCompany;
        public virtual AugmentedCompany Company
        {
            get { return fCompany; }
            set { fCompany = value; }
        }
        [Aggregated]
        public virtual List<AccountingResultByComponent> AccountingResultByComponents { get; set; }

        public int Year { get; set; }
    }
}
