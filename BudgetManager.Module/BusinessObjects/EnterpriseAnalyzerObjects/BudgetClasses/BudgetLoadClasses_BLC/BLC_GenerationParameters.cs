using DevExpress.Persistent.Base;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace BudgetManager.Module.BusinessObjects.EnterpriseAnalyzerObjects
{
    [DefaultClassOptions]
    [NavigationItem("Z-BudgetLoadClass")]
    [DefaultProperty("Name")]
    public class BLC_GenerationParameters : CompanyDataObject
    {
        [Browsable(false)]
        public Int32 GenerationParametersId { get; set; }
        public BLC_GenerationParameters() { }
        public string Name { get; set; }

        public BudgetTreeNode BudgetTreeNode { get; set; }

        [NotMapped]
        public string Account
        {
            get { return BudgetTreeNode.Name == null ? "" : BudgetTreeNode.Name; }
        }

        public BLC_GenerationType GenerationType { get; set; }

        public BLC_PaymentPlan PaymentPlan { get; set; }

        public int IncrementModulus { get; set; }

        public string ReplicationMode { get; set; }

        public decimal PercentageIncrease { get; set; }

        public decimal ValueIncrease { get; set; }

        public string IncrementPriority { get; set; }

        //Hay que volverlo a poner
        ////public Currency Currency { get; set; }
    }
}
