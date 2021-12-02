using DevExpress.Persistent.Base;
using System;
using System.ComponentModel;

namespace BudgetManager.Module.BusinessObjects.EnterpriseAnalyzerObjects
{
    [DefaultClassOptions]
    [NavigationItem("ActivityCenters -")]
    [System.ComponentModel.DefaultProperty("Name")]
    public class ActivityCenterTreeNode : CompanyTreeNode
    {
        public ActivityCenterTreeNode() { }

        //[Browsable(false)]
        //public Int32 ID { get; protected set; }
        [Browsable(false)]
        public Int32 ActivityCenterTreeNodeId { get; protected set; }

        public bool IsActivityCenterHeadNode { get; set; }

        public virtual ActivityCenterNode ActivityCenterNode { get; set; }



    }
}
