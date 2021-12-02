using DevExpress.Persistent.Base;
using System;
using System.ComponentModel;

namespace BudgetManager.Module.BusinessObjects.EnterpriseAnalyzerObjects
{
    [DefaultClassOptions]
    [NavigationItem("Z-BudgetLoadClass")]
    [System.ComponentModel.DefaultProperty("Name")]
    public class BLC_PaymentPlan : CompanyDataObject
    {
        [Browsable(false)]
        public Int32 BLC_GenerationTypeId { get; set; }

        public string Name { get; set; }
    }
}
