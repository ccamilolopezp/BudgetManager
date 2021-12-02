using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using System;
using System.ComponentModel;

namespace BudgetManager.Module.BusinessObjects.EnterpriseAnalyzerObjects
{

    [NavigationItem("Z-Budgets -")]
    [System.ComponentModel.DefaultProperty("Description")]
    [DefaultClassOptions]
    public class ComponentPeriodBudgetItem : BudgetBaseObject, IXafEntityObject, IObjectSpaceLink
    {
        [Browsable(false)]
        public Int32 ID { get; protected set; }

        private ComponentPeriodBudget fComponentPeriodBudget;
        public virtual ComponentPeriodBudget ComponentPeriodBudget
        {
            get { return fComponentPeriodBudget; }
            set { fComponentPeriodBudget = value; }
        }
        public bool IsLoading { get; set; }


        private decimal fAmount;

        public string LotID { get; set; }

        public string BaseGeneratorID { get; set; }

        public string Description { get; set; }

        //Me falta manejar la situación en que este campo cambia de valor.
        //Creo que ya lo hace con el OnSaving, pero no sé porque en XPO era distinto 
        public decimal Amount { get; set; }

        public string Project { get; set; }

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
