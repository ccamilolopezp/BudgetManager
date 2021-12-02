using BudgetManager.Module.BusinessObjects.EnterpriseAnalyzerObjects;
using DevExpress.ExpressApp;
using System;
using System.Linq;

namespace BudgetManager.Module.BusinessManagers.EnterpriseAnalyzerManagers
{
    class FetchBudgetTreeNodeClass
    {
        public BudgetTreeNode FetchBudgetTreeNodeByNameOrLabel(IObjectSpace ios,
                                                               string budgetTreeNodeNameOrLabel,
                                                               Company company,
                                                               string callerMethodName)
        {
            BudgetTreeNode budgetTreeNode = FetchBudgetTreeNodeByNameOrLabel(ios,
                                                                 budgetTreeNodeNameOrLabel,
                                                                 company.Name,
                                                                 callerMethodName);

            return budgetTreeNode;
        }

        public BudgetTreeNode FetchBudgetTreeNodeByNameOrLabel(IObjectSpace ios,
                                                               string budgetTreeNodeNameOrLabel,
                                                               string companyName,
                                                               string callerMethodName)
        {
            BudgetTreeNode budgetTreeNode = ios.GetObjectsQuery<BudgetTreeNode>()
                                            .Where(o => o.BudgetTree.CompanyName == companyName &
                                                                                    (o.Label == budgetTreeNodeNameOrLabel | o.Name == budgetTreeNodeNameOrLabel))
                                            .FirstOrDefault();

            if (budgetTreeNode == null)
            {
                throw new Exception("("
                                    + callerMethodName
                                    + ") No existe BudgetTreeNode: "
                                    + budgetTreeNodeNameOrLabel
                                    + " , "
                                    + companyName);
            }

            return budgetTreeNode;
        }
    }
}
