using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace BudgetManager.Module.BusinessObjects.EnterpriseAnalyzerObjects
{
    [NavigationItem("Z-Budgets -")]
    [System.ComponentModel.DefaultProperty("BudgetTreeNode.Name")]
    [DefaultClassOptions]
    public class ComponentBudget : BudgetBaseObject, IXafEntityObject, IObjectSpaceLink
    {
        [Browsable(false)]
        public Int32 ComponentBudgetId { get; protected set; }

        private CompanyYearBudget fCompanyYearBudget;
        public virtual CompanyYearBudget CompanyYearBudget
        {
            get { return fCompanyYearBudget; }
            set { fCompanyYearBudget = value; }
        }

        public ComponentBudget()
        {
            ComponentPeriodBudgets = new List<ComponentPeriodBudget>();
        }

        [Aggregated]
        public virtual List<ComponentPeriodBudget> ComponentPeriodBudgets { get; set; }
        public bool IsLoading { get; set; }
        public virtual ActivityCenterNode ActivityCenterNode { get; set; }
        public virtual BudgetTreeNode BudgetTreeNode { get; set; }
        public string Description { get; set; }

        private Decimal fYearAmount { get; set; }
        public decimal YearAmount
        {
            get { return fYearAmount; }
            protected set { fYearAmount = value; }
        }

        private Decimal fYearExecutedBudget { get; set; }
        public decimal YearExecutedBudget
        {
            get { return fYearExecutedBudget; }
            protected set { fYearExecutedBudget = value; }
        }
        private int fMaximumPeriodWithExecutionValues { get; set; }
        public int MaximumPeriodWithExecutionValues
        {
            get { return fMaximumPeriodWithExecutionValues; }
            protected set { fMaximumPeriodWithExecutionValues = value; }
        }

        [NotMapped]
        public decimal YearRemainingBudget
        {
            get { return YearAmount - YearExecutedBudget; }
        }

        public void UpdateComponentBudgetStatus(bool forceChangeEvents)
        {
            Decimal oldYearAmount = fYearAmount;
            Decimal oldYearExecutedBudget = fYearExecutedBudget;
            int oldMaximumPeriodWithExecutionValues = fMaximumPeriodWithExecutionValues;

            decimal tempExecutedBudget = 0.0m;

            decimal tempAmount = 0.0m;

            int maximumPeriodWithExecutionValues = 0;

            foreach (ComponentPeriodBudget componentPeriodBudget in ComponentPeriodBudgets)
            {
                tempAmount += componentPeriodBudget.PeriodAmount;

                if (componentPeriodBudget.HasExecutionValues)
                {
                    tempExecutedBudget += componentPeriodBudget.PeriodExecutedBudget;
                    maximumPeriodWithExecutionValues = Math.Max(maximumPeriodWithExecutionValues, componentPeriodBudget.BudgetPeriod.Consecutive);
                }
            }

            fYearAmount = tempAmount;
            fYearExecutedBudget = tempExecutedBudget;
            fMaximumPeriodWithExecutionValues = maximumPeriodWithExecutionValues;

            if (forceChangeEvents)
            {
                YearAmount = fYearAmount;
                YearExecutedBudget = fYearExecutedBudget;
                MaximumPeriodWithExecutionValues = fMaximumPeriodWithExecutionValues;
            }
        }

        #region IXafEntityObject members

        void IXafEntityObject.OnCreated()
        {
            if (objectSpace != null)
            {
                if (fCompanyYearBudget != null & !IsLoading)
                {
                    fCompanyYearBudget.UpdateCompanyYearBudgetStatus(true);
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
                if (fCompanyYearBudget != null & !IsLoading)
                {
                    fCompanyYearBudget.UpdateCompanyYearBudgetStatus(true);
                }
                else
                {
                    if (CompanyYearBudget != null & !IsLoading)
                    {
                        CompanyYearBudget.UpdateCompanyYearBudgetStatus(true);
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
