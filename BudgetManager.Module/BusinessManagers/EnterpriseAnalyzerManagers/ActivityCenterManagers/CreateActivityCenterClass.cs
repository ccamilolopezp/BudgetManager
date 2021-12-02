using BudgetManager.Module.BusinessObjects.EnterpriseAnalyzerObjects;
using DevExpress.ExpressApp;

namespace BudgetManager.Module.BusinessManagers.EnterpriseAnalyzerManagers
{
    class CreateActivityCenterClass
    {
        public void CreateDefaultNewActivityCenter(IObjectSpace ios,
                                                   CompanyTreeNode companyTreeNode,
                                                   CompanyTree companyTree)
        {
            ActivityCenterTreeNode activityCenterTreeNode = ios.CreateObject<ActivityCenterTreeNode>();
            activityCenterTreeNode.CompanyTree = companyTree;
            activityCenterTreeNode.Parent = companyTreeNode;
            activityCenterTreeNode.IsActivityCenterHeadNode = true;
            activityCenterTreeNode.IsActivityCenterNode = true;
            activityCenterTreeNode.Name = companyTreeNode.Name;
            activityCenterTreeNode.Label = companyTreeNode.Label;

            ActivityCenterNode activityCenterNode = ios.CreateObject<ActivityCenterNode>();
            activityCenterNode.ActivityCenterTreeNode = activityCenterTreeNode;
            activityCenterNode.CompanyName = activityCenterTreeNode.CompanyTree.Company.Name;
            //activityCenterNode.CompanyTreeNode = structureInternalNode.StructureToTreeNodeLink.CompanyTreeNode;

        }
    }
}
