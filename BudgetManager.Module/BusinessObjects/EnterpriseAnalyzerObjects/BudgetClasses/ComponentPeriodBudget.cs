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
    [System.ComponentModel.DefaultProperty("ComponentBudget.BudgetTreeNode.Name")]
    [DefaultClassOptions]
    public class ComponentPeriodBudget : BudgetBaseObject, IXafEntityObject, IObjectSpaceLink
    {
        [Browsable(false)]
        public Int32 ComponentPeriodBudgetId { get; protected set; }

        private ComponentBudget fComponentBudget;
        public virtual ComponentBudget ComponentBudget
        {
            get { return fComponentBudget; }
            set { fComponentBudget = value; }
        }

        public ComponentPeriodBudget()
        {
            ComponentPeriodBudgetItems = new List<ComponentPeriodBudgetItem>();
            LinkToBudgets = new List<LinkToBudget>();
        }

        [Aggregated]
        public virtual List<ComponentPeriodBudgetItem> ComponentPeriodBudgetItems { get; set; }
        [Aggregated]
        public virtual List<LinkToBudget> LinkToBudgets { get; set; }
        public bool IsLoading { get; set; }

        private Decimal fPeriodAmount { get; set; }
        public decimal PeriodAmount
        {
            get { return fPeriodAmount; }
            protected set { fPeriodAmount = value; }
        }

        private Decimal fPeriodExecutedBudget { get; set; }
        public decimal PeriodExecutedBudget
        {
            get { return fPeriodExecutedBudget; }
            protected set { fPeriodExecutedBudget = value; }
        }
        private bool fHasExecutionValues { get; set; }
        public bool HasExecutionValues
        {
            get { return fHasExecutionValues; }
            protected set { fHasExecutionValues = value; }
        }

        [NotMapped]
        public decimal PeriodRemainingBudget
        {
            get { return PeriodAmount - PeriodExecutedBudget; }
        }

        public virtual BudgetPeriod BudgetPeriod { get; set; }

        public void UpdateComponentPeriodBudgetStatus(bool forceChangeEvents)
        {
            decimal oldPeriodAmount = fPeriodAmount;

            decimal oldPeriodExecutedBudget = fPeriodExecutedBudget;

            bool oldHasExecutionValues = fHasExecutionValues;

            decimal tempPeriodExecutedBudget = 0.0m;

            if (LinkToBudgets.Count == 0)
            {
                fHasExecutionValues = false;
            }
            else
            {
                fHasExecutionValues = true;

                foreach (LinkToBudget linkToBudget in LinkToBudgets)
                {
                    tempPeriodExecutedBudget += linkToBudget.PeriodAmount;
                }
            }

            decimal tempPeriodAmount = 0.0m;

            foreach (ComponentPeriodBudgetItem componentPeriodBudgetItem in ComponentPeriodBudgetItems)
            {
                tempPeriodAmount += componentPeriodBudgetItem.Amount;
            }

            fPeriodAmount = tempPeriodAmount;
            fPeriodExecutedBudget = tempPeriodExecutedBudget;

            if (forceChangeEvents)
            {
                PeriodAmount = fPeriodAmount;

                PeriodExecutedBudget = fPeriodExecutedBudget;

                HasExecutionValues = fHasExecutionValues;

            }
        }

        #region IXafEntityObject members

        void IXafEntityObject.OnCreated()
        {
            if (objectSpace != null)
            {
                if (fComponentBudget != null & !IsLoading)
                {
                    fComponentBudget.UpdateComponentBudgetStatus(true);
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
                if (fComponentBudget != null & !IsLoading)
                {
                    fComponentBudget.UpdateComponentBudgetStatus(true);
                }
                else
                {
                    if (ComponentBudget != null & !IsLoading)
                    {
                        ComponentBudget.UpdateComponentBudgetStatus(true);
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
