using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace BudgetManager.Module.BusinessObjects.EnterpriseAnalyzerObjects
{
    [NavigationItem("Z-Accounting")]
    [System.ComponentModel.DefaultProperty("BudgetTreeNode.Name")]
    [DefaultClassOptions]
    public class AccountingResultByComponent : CompanyDataObject
    {
        [Browsable(false)]
        public Int32 AccountingResultByComponentId { get; protected set; }

        public AccountingResultByComponent()
        {
            AccountingResultByComponentByPeriods = new List<AccountingResultByComponentByPeriod>();
        }

        private CompanyYearAccountingResult fCompanyYearAccountingResult;
        public virtual CompanyYearAccountingResult CompanyYearAccountingResult
        {
            get { return fCompanyYearAccountingResult; }
            set { fCompanyYearAccountingResult = value; }
        }
        [Aggregated]
        public virtual List<AccountingResultByComponentByPeriod> AccountingResultByComponentByPeriods { get; set; }

        public virtual ActivityCenterNode ActivityCenterNode { get; set; }

        ////Cambio a ValueModelTree
        ////public virtual AccountingPlanTreeNode AccountingPlanTreeNode { get; set; }
        public virtual ValueModelTreeNode AccountingPlanTreeNode { get; set; }

        public string Description { get; set; }

        private Decimal fYearAmount { get; set; }
        public decimal YearAmount
        {
            get { return fYearAmount; }
            protected set { fYearAmount = value; }
        }

        public void UpdateAccountingResultByComponentStatus(bool forceChangeEvents)
        {
            decimal oldYearAmount = fYearAmount;

            decimal tempExecutedBudget = 0.0m;

            //foreach (LinkToBudget linkToBudget in LinkToBudgets)
            //{
            //    tempExecutedBudget += linkToBudget.CashMovementItem.Amount;
            //}

            decimal tempAmount = 0.0m;

            foreach (AccountingResultByComponentByPeriod accountingResultByComponentByPeriod in AccountingResultByComponentByPeriods)
            {
                tempAmount += accountingResultByComponentByPeriod.PeriodAmount;
            }

            fYearAmount = tempAmount;

            if (forceChangeEvents)
            {
                YearAmount = fYearAmount;
            }
        }

        /// <summary>
        /// ////////////////////////////////////
        /// </summary>

    }
}
