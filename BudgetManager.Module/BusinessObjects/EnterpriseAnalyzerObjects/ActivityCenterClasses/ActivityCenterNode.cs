using DevExpress.Persistent.Base;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace BudgetManager.Module.BusinessObjects.EnterpriseAnalyzerObjects
{
    [NavigationItem("ActivityCenters -")]
    [DefaultClassOptions]
    [System.ComponentModel.DefaultProperty("Name")]
    public class ActivityCenterNode : ActivityCenterBaseObject
    {
        [Browsable(false)]
        [ForeignKey("ActivityCenterTreeNode")]
        public Int32 ActivityCenterNodeId { get; protected set; }
        public virtual ActivityCenterTreeNode ActivityCenterTreeNode { get; set; }

        private String eRPCode { get; set; }


        [NotMapped]
        public string Name
        {
            get { return ActivityCenterTreeNode.Name == null ? "" : ActivityCenterTreeNode.Name + "-" + ActivityCenterTreeNode.Label; }
        }

        //De pronto en el futuro
        ////[Association("ActivityCenterNode-BillingConceptGroups")]
        ////public XPCollection<BillingConceptGroup> BillingConceptGroups
        ////{
        ////    get { return GetCollection<BillingConceptGroup>("BillingConceptGroups"); }
        ////}

        ////[Association("ActivityCenterNode-ServiceOrders")]
        ////public XPCollection<ServiceOrder> ServiceOrders
        ////{
        ////    get { return GetCollection<ServiceOrder>("ServiceOrders"); }
        ////}


    }
}
