using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.BaseImpl.EF;
using System;
using System.ComponentModel;

namespace BudgetManager.Module.BusinessObjects.NonPersistent
{
    [DomainComponent]
    public class LoadTreeParameters
    {
        [Browsable(false)]
        public BindingList<Budget.XAF.Model.Sql.TreeName> _TreeName;

        [CollectionOperationSet(AllowAdd = true, AllowRemove = true)]
        public BindingList<Budget.XAF.Model.Sql.TreeName> TreesToProcess
        {
            get
            {
                if (_TreeName == null)
                {
                    _TreeName = new BindingList<Budget.XAF.Model.Sql.TreeName>
                    {
                        AllowEdit = true,
                        AllowRemove = true
                    };
                }
                return _TreeName;
            }

        }
        public bool LoadFormulation { get; set; }
        public FileData FormulaData { get; set; }
        public FileData StructuralData { get; set; }
    }

    [DomainComponent]
    public class LoadBudgetTreeParameters
    {
        public FileData BudgetData { get; set; }
        public Budget.Model.Sql.LoadParameters LoadParameters { get; set; }
    }

    [DomainComponent]
    public class LoadBudgetValuesParameters
    {
        public Budget.Model.Sql.LoadParameters LoadParameters { get; set; }
        public DateTime InitialDate { get; set; }
        public DateTime FinalDate { get; set; }
        public FileData BudgetData { get; set; }
    }

    [DomainComponent]
    public class LoadExecutedBudgetValuesParameters
    {
        public Budget.Model.Sql.LoadParameters LoadParameters { get; set; }
        public DateTime ProcessDate { get; set; }
        public FileData BudgetData { get; set; }
    }

    [DomainComponent]
    public class LinkValuesParameters
    {
        public Budget.Model.Sql.BudgetUnit BudgetUnit { get; set; }
        public Budget.Model.Sql.ExecutedBudgetUnit ExecutedBudgetUnit { get; set; }
    }
}