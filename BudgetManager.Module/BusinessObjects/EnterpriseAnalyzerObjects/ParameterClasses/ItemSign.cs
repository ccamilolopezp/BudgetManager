using DevExpress.Persistent.Base;
using System;
using System.ComponentModel;

namespace BudgetManager.Module.BusinessObjects.EnterpriseAnalyzerObjects
{
    [NavigationItem("Parameter -")]
    [DefaultClassOptions]
    [DefaultProperty("Name")]
    public class ItemSign : ParameterBaseObject
    {
        public ItemSign() { }

        //[Browsable(false)]
        //public Int32 ID { get; protected set; }
        [Browsable(false)]
        public Int32 CompanyId { get; protected set; }

        public string Name { get; set; }

        public bool IsPositive { get; set; }

        public bool IsDual { get; set; }

        public decimal Multiplier { get; set; }
    }
}
