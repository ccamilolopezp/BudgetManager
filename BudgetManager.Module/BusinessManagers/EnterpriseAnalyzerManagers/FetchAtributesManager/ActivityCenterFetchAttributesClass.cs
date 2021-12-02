using BudgetManager.Module.BusinessObjects.EnterpriseAnalyzerObjects;
using System;
using System.Linq;

namespace BudgetManager.Module.BusinessManagers.EnterpriseAnalyzerManagers
{
    class ActivityCenterFetchAttributesClass
    {
        public CompanyTreeNode FetchActivityCenterStructureTreeNode(ActivityCenterNode activityCenterNode,
                                                                    string callerMethodName)
        {
            CompanyTreeNode companyTreeNode =
                (from stn in activityCenterNode.ActivityCenterTreeNode.CompanyTreeNodeAncestors
                 where stn.AncestorCompanyTreeNode.IsEndNode
                 select stn.AncestorCompanyTreeNode).FirstOrDefault();


            if (companyTreeNode == null)
            {
                throw new Exception("("
                                    + callerMethodName
                                    + ") No existe CompanyTreeNode para ActvityCenterNode: "
                                    + activityCenterNode.Name);
            }
            else
            {
                return companyTreeNode;
            }


        }


    }
}
