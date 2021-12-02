using DevExpress.Persistent.Base;
using System;
using System.ComponentModel;

namespace BudgetManager.Module.BusinessObjects.EnterpriseAnalyzerObjects
{
    [DefaultClassOptions]
    [NavigationItem("Z-BudgetLoadClass")]
    [DefaultProperty("ID")]
    public class BLC_GenerationType : CompanyDataObject
    {
        [Browsable(false)]
        public Int32 BLC_GenerationTypeId { get; set; }
        public BLC_GenerationType() { }
        public string Name { get; set; }

        public string PaymentMode { get; set; }

        public BLC_TaxManagement TaxManagement { get; set; }
    }
}
