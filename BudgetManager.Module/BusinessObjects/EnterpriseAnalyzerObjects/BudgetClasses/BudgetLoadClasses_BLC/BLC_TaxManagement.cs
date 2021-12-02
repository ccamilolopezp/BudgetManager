using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace BudgetManager.Module.BusinessObjects.EnterpriseAnalyzerObjects
{
    [DefaultClassOptions]
    [NavigationItem("Z-BudgetLoadClass")]
    [DefaultProperty("Name")]
    public class BLC_TaxManagement : CompanyDataObject
    {
        public BLC_TaxManagement()
        {
            BLC_TaxManagementItems = new List<BLC_TaxManagementItem>();
        }

        [Aggregated]
        public virtual IList<BLC_TaxManagementItem> BLC_TaxManagementItems { get; set; }

        [Browsable(false)]
        public Int32 BLC_TaxManagementId { get; protected set; }

        public string Name { get; set; }
    }

    [DefaultClassOptions]
    [NavigationItem("Z-BudgetLoadClass")]
    public class BLC_TaxManagementItem : CompanyDataObject
    {
        public virtual BLC_TaxManagement TaxManagement { get; set; }
        public BLC_TaxManagementItem() { }

        //[Browsable(false)]
        //public Int32 ID { get; protected set; }
        [Browsable(false)]
        public Int32 BLC_TaxManagementItemId { get; protected set; }

        public int Precedence { get; set; }

        public decimal Rate { get; set; }

        public string DocumentType { get; set; }

        public BLC_TaxType TaxType { get; set; }
    }

    [DefaultClassOptions]
    [NavigationItem("Z-BudgetLoadClass")]
    public class BLC_TaxType : CompanyDataObject
    {
        public BLC_TaxType() { }

        //[Browsable(false)]
        //public Int32 ID { get; protected set; }
        [Browsable(false)]
        public Int32 BLC_TaxTypeId { get; protected set; }
        private string name;
        private decimal rate;
        private ItemSign relativeSign;

        public string Name { get; set; }

        public decimal Rate { get; set; }

        public ItemSign RelativeSign { get; set; }
    }

}
