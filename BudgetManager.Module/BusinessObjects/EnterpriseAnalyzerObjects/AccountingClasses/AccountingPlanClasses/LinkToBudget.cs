using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;


namespace BudgetManager.Module.BusinessObjects.EnterpriseAnalyzerObjects
{
    [DefaultClassOptions]
    [NavigationItem("Z-Accounting")]
    [System.ComponentModel.DefaultProperty("BudgetPeriodName")]
    public class LinkToBudget : CompanyDataObject, IXafEntityObject, IObjectSpaceLink
    {

        [ForeignKey("AccountingResultByComponentByPeriod")]
        [Browsable(false)]
        public Int32 LinkToBudgetId { get; protected set; }

        private ComponentPeriodBudget fComponentPeriodBudget;
        public virtual ComponentPeriodBudget ComponentPeriodBudget
        {
            get { return fComponentPeriodBudget; }
            set { fComponentPeriodBudget = value; }
        }

        public bool IsLoading { get; set; }

        [NotMapped]
        public decimal PeriodAmount
        {
            get { return AccountingResultByComponentByPeriod == null ? 0.0m : AccountingResultByComponentByPeriod.PeriodAmount; }
        }

        [NotMapped]
        public string BudgetPeriodName
        {
            get { return ComponentPeriodBudget == null ? "" : ComponentPeriodBudget.BudgetPeriod.Name; }
        }

        public virtual AccountingResultByComponentByPeriod AccountingResultByComponentByPeriod { get; set; }

        #region IXafEntityObject members

        void IXafEntityObject.OnCreated()
        {
            if (objectSpace != null)
            {
                if (fComponentPeriodBudget != null & !IsLoading)
                {
                    fComponentPeriodBudget.UpdateComponentPeriodBudgetStatus(true);
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
                if (fComponentPeriodBudget != null & !IsLoading)
                {
                    fComponentPeriodBudget.UpdateComponentPeriodBudgetStatus(true);
                }
                else
                {
                    if (ComponentPeriodBudget != null & !IsLoading)
                    {
                        ComponentPeriodBudget.UpdateComponentPeriodBudgetStatus(true);
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
