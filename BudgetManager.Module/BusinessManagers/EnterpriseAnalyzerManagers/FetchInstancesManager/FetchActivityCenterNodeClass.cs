using BudgetManager.Module.BusinessObjects.EnterpriseAnalyzerObjects;
using DevExpress.ExpressApp;
using System;
using System.Linq;

namespace BudgetManager.Module.BusinessManagers.EnterpriseAnalyzerManagers
{
    class FetchActivityCenterNodeClass
    {
        public ActivityCenterNode FetchActivityCenterNode(IObjectSpace ios,
                                                          string activityCenterNodeName,
                                                          Company company,
                                                          string callerMethodName)
        {
            ActivityCenterNode activityCenterNode = ios.GetObjectsQuery<ActivityCenterNode>()
                                                    .Where(o => o.CompanyName == company.Name & (o.ActivityCenterTreeNode.Name == activityCenterNodeName
                                                                                                  | o.ActivityCenterTreeNode.Label == activityCenterNodeName))
                                                    .FirstOrDefault();

            //List<ActivityCenterNode> lactivityCenterNode = ios.GetObjects<ActivityCenterNode>()
            //                                        .Where(o => o.CompanyName == company.Name)
            //                                        .ToList();
            //List<ActivityCenterNode> l2activityCenterNode = ios.GetObjects<ActivityCenterNode>()
            //                                        .Where(o => o.ActivityCenterTreeNode.Label == activityCenterNodeLabel)
            //                                        .ToList();
            if (activityCenterNode == null)
            {
                throw new Exception("("
                                    + callerMethodName
                                    + ") No existe ActivityCenterNode: "
                                    + activityCenterNodeName
                                    + " , "
                                    + company.Name);
            }

            return activityCenterNode;
        }
    }
}
