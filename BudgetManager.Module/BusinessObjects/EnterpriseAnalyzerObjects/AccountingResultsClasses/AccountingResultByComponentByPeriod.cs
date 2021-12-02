using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace BudgetManager.Module.BusinessObjects.EnterpriseAnalyzerObjects
{
    [NavigationItem("Z-Accounting")]
    [DefaultProperty("AccountingPeriod.Name")]
    [DefaultClassOptions]
    public class AccountingResultByComponentByPeriod : CompanyDataObject, IXafEntityObject, IObjectSpaceLink
    {
        //[ForeignKey("LinkToBudget")]
        [Browsable(false)]
        public Int32 AccountingResultByComponentByPeriodId { get; protected set; }

        public AccountingResultByComponentByPeriod()
        {
            AccountingResultItems = new List<AccountingResultItem>();
        }

        private AccountingResultByComponent fAccountingResultByComponent;
        public virtual AccountingResultByComponent AccountingResultByComponent
        {
            get { return fAccountingResultByComponent; }
            set { fAccountingResultByComponent = value; }
        }
        [Aggregated]
        public virtual List<AccountingResultItem> AccountingResultItems { get; set; }
        public virtual LinkToBudget LinkToBudget { get; set; }
        public bool IsLoading { get; set; }

        public virtual BudgetPeriod AccountingPeriod { get; set; }
        //public virtual AccountingPlanTreeNode AccountingPlanTreeNode { get; set; }

        public string Description { get; set; }

        public DateTime EnOfPeriodDate { get; set; }

        public Decimal OpeningBalance { get; set; }
        private Decimal fPeriodAmount { get; set; }
        public decimal PeriodAmount
        {
            get { return fPeriodAmount; }
            protected set { fPeriodAmount = value; }
        }
        private Decimal fDebitAmount { get; set; }
        public decimal DebitAmount
        {
            get { return fDebitAmount; }
            protected set { fDebitAmount = value; }
        }
        private Decimal fCreditAmount { get; set; }
        public decimal CreditAmount
        {
            get { return fCreditAmount; }
            protected set { fCreditAmount = value; }
        }
        public Decimal ClosingBalance { get; set; }

        private Decimal fCalculatedClosingBalance { get; set; }
        public decimal CalculatedClosingBalance
        {
            get { return fCalculatedClosingBalance; }
            protected set { fCalculatedClosingBalance = value; }
        }
        public string AccountCode { get; set; }

        public void UpdateAccountingResultByComponentByPeriodStatus(bool forceChangeEvents)
        {
            decimal tempPeriodAmount = 0.0m;
            decimal tempDebitAmount = 0.0m;
            decimal tempCreditAmount = 0.0m;
            decimal multiplier = 0.0m;
            decimal creditMultiplier = 0.0m;

            foreach (AccountingResultItem accountingResultItem in AccountingResultItems)
            {
                if (Regex.Match(this.AccountingResultByComponent.AccountingPlanTreeNode.Name, "^-PC-1").Success)
                {
                    if (accountingResultItem.AccountingResultItemType.IsDebit)
                    {
                        tempDebitAmount += accountingResultItem.Amount;
                        multiplier = 1.0m;
                    }
                    else
                    {
                        tempCreditAmount += accountingResultItem.Amount;
                        multiplier = -1.0m;
                    }
                }

                if (Regex.Match(this.AccountingResultByComponent.AccountingPlanTreeNode.Name, "^-PC-2").Success)
                {
                    if (accountingResultItem.AccountingResultItemType.IsDebit)
                    {
                        tempDebitAmount += accountingResultItem.Amount;
                        multiplier = -1.0m;
                    }
                    else
                    {
                        tempCreditAmount += accountingResultItem.Amount;
                        multiplier = 1.0m;
                    }
                }

                if (Regex.Match(this.AccountingResultByComponent.AccountingPlanTreeNode.Name, "^-PC-3").Success)
                {
                    if (accountingResultItem.AccountingResultItemType.IsDebit)
                    {
                        tempDebitAmount += accountingResultItem.Amount;
                        multiplier = -1.0m;
                    }
                    else
                    {
                        tempCreditAmount += accountingResultItem.Amount;
                        multiplier = 1.0m;
                    }
                }

                if (Regex.Match(this.AccountingResultByComponent.AccountingPlanTreeNode.Name, "^-PC-4").Success)
                {
                    if (accountingResultItem.AccountingResultItemType.IsDebit)
                    {
                        tempDebitAmount += accountingResultItem.Amount;
                        multiplier = -1.0m;
                    }
                    else
                    {
                        tempCreditAmount += accountingResultItem.Amount;
                        multiplier = 1.0m;
                    }
                }

                if (Regex.Match(this.AccountingResultByComponent.AccountingPlanTreeNode.Name, "^-PC-5").Success)
                {
                    if (accountingResultItem.AccountingResultItemType.IsDebit)
                    {
                        tempDebitAmount += accountingResultItem.Amount;
                        multiplier = 1.0m;
                    }
                    else
                    {
                        tempCreditAmount += accountingResultItem.Amount;
                        multiplier = -1.0m;
                    }
                }

                if (Regex.Match(this.AccountingResultByComponent.AccountingPlanTreeNode.Name, "^-PC-7").Success)
                {
                    if (accountingResultItem.AccountingResultItemType.IsDebit)
                    {
                        tempDebitAmount += accountingResultItem.Amount;
                        multiplier = 1.0m;
                    }
                    else
                    {
                        tempCreditAmount += accountingResultItem.Amount;
                        multiplier = -1.0m;
                    }
                }
                tempPeriodAmount += multiplier * accountingResultItem.Amount;
            }

            fPeriodAmount = tempPeriodAmount;
            fDebitAmount = tempDebitAmount;
            fCreditAmount = tempCreditAmount;
            fCalculatedClosingBalance = this.OpeningBalance + tempPeriodAmount;

            if (forceChangeEvents)
            {
                PeriodAmount = fPeriodAmount;
                DebitAmount = fDebitAmount;
                CreditAmount = fCreditAmount;
                CalculatedClosingBalance = fCalculatedClosingBalance;
            }
        }

        #region IXafEntityObject members

        void IXafEntityObject.OnCreated()
        {
            if (objectSpace != null)
            {
                if (fAccountingResultByComponent != null & !IsLoading)
                {
                    fAccountingResultByComponent.UpdateAccountingResultByComponentStatus(true);
                }

            }
        }
        void IXafEntityObject.OnLoaded()
        {
            //IsNew = false;
        }

        void IXafEntityObject.OnSaving()
        {
            if (objectSpace != null)
            {
                if (fAccountingResultByComponent != null & !IsLoading)
                {
                    fAccountingResultByComponent.UpdateAccountingResultByComponentStatus(true);
                }
                else
                {
                    if (AccountingResultByComponent != null & !IsLoading)
                    {
                        AccountingResultByComponent.UpdateAccountingResultByComponentStatus(true);
                    }
                }
            }
            //IsNew = false;
        }
        #endregion
        #region IObjectSpaceLink members
        private IObjectSpace objectSpace;
        IObjectSpace IObjectSpaceLink.ObjectSpace
        {
            get { return objectSpace; }
            set { objectSpace = value; }
        }
        #endregion


    }


}
