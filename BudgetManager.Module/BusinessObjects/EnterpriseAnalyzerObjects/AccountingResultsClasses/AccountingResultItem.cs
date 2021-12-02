using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using System;
using System.ComponentModel;

namespace BudgetManager.Module.BusinessObjects.EnterpriseAnalyzerObjects
{
    [NavigationItem("Z-Accounting")]
    [System.ComponentModel.DefaultProperty("Description")]
    [DefaultClassOptions]
    public class AccountingResultItem : CompanyDataObject, IXafEntityObject, IObjectSpaceLink
    {
        [Browsable(false)]
        public Int32 AccountingResultItemId { get; protected set; }

        public AccountingResultItem() { }

        private AccountingResultByComponentByPeriod fAccountingResultByComponentByPeriod;
        public virtual AccountingResultByComponentByPeriod AccountingResultByComponentByPeriod
        {
            get { return fAccountingResultByComponentByPeriod; }
            set { fAccountingResultByComponentByPeriod = value; }
        }

        public bool IsLoading { get; set; }
        public string LotID { get; set; }
        public string BaseGeneratorID { get; set; }

        public string Description { get; set; }

        public string FullAccountCode { get; set; }

        public virtual AccountingResultItemType AccountingResultItemType { get; set; }

        public decimal Amount { get; set; }

        public string Project { get; set; }

        #region IXafEntityObject members

        void IXafEntityObject.OnCreated()
        {
            if (objectSpace != null)
            {
                if (fAccountingResultByComponentByPeriod != null & !IsLoading)
                {
                    fAccountingResultByComponentByPeriod.UpdateAccountingResultByComponentByPeriodStatus(true);
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
                if (fAccountingResultByComponentByPeriod != null & !IsLoading)
                {
                    fAccountingResultByComponentByPeriod.UpdateAccountingResultByComponentByPeriodStatus(true);
                }
                else
                {
                    if (AccountingResultByComponentByPeriod != null & !IsLoading)
                    {
                        AccountingResultByComponentByPeriod.UpdateAccountingResultByComponentByPeriodStatus(true);
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
