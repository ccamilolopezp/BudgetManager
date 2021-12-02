using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using System.ComponentModel;

namespace BudgetManager.Module.BusinessObjects.NonPersistent
{
    [DomainComponent]
    public class DeleteParameters
    {
        [Browsable(false)]
        public BindingList<Budget.Model.Sql.BudgetUnit> _BudgetUnits;

        [CollectionOperationSet(AllowAdd = true, AllowRemove = true)]
        public BindingList<Budget.Model.Sql.BudgetUnit> BudgetUnitsToDelete
        {
            get
            {
                if (_BudgetUnits == null)
                {
                    _BudgetUnits = new BindingList<Budget.Model.Sql.BudgetUnit>
                    {
                        AllowEdit = true,
                        AllowRemove = true
                    };
                }
                return _BudgetUnits;
            }

        }

        [Browsable(false)]
        public BindingList<Budget.Model.Sql.ExecutedBudgetUnit> _ExecutedBudgetUnits;

        [CollectionOperationSet(AllowAdd = true, AllowRemove = true)]
        public BindingList<Budget.Model.Sql.ExecutedBudgetUnit> ExecutedBudgetUnitsToDelete
        {
            get
            {
                if (_ExecutedBudgetUnits == null)
                {
                    _ExecutedBudgetUnits = new BindingList<Budget.Model.Sql.ExecutedBudgetUnit>
                    {
                        AllowEdit = true,
                        AllowRemove = true
                    };
                }
                return _ExecutedBudgetUnits;
            }

        }

    }
}
