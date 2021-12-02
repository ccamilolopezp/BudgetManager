using BudgetManager.Module.BusinessObjects.EnterpriseAnalyzerObjects;
using DevExpress.ExpressApp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BudgetManager.Module.BusinessManagers.EnterpriseAnalyzerManagers
{
    class FetchAccountingResultByComponentClass
    {
        public AccountingResultByComponent FetchOrCreateAccountingResultByComponent(IObjectSpace ios,
                                                                                    CompanyYearAccountingResult companyYearAccountingResult,
                                                                                    string activityCenterNodeName,
                                                                                    string accountingPlanTreeNodeName,
                                                                                    string accountingPlanTreeNodeLabel,
                                                                                    string callerMethodName)
        {
            AccountingResultByComponent accountingResultByComponent = ios.GetObjectsQuery<AccountingResultByComponent>()
                                                                      .Where(arbc => arbc.CompanyYearAccountingResult.CompanyYearAccountingResultId == companyYearAccountingResult.CompanyYearAccountingResultId
                                                                                     && arbc.ActivityCenterNode.ActivityCenterTreeNode.Name == activityCenterNodeName
                                                                                     && arbc.AccountingPlanTreeNode.Name == accountingPlanTreeNodeName)
                                                                      .FirstOrDefault();

            if (accountingResultByComponent == null)
            {
                ActivityCenterNode activityCenterNode =
                    (new FetchActivityCenterNodeClass()).FetchActivityCenterNode(ios, activityCenterNodeName, companyYearAccountingResult.Company, callerMethodName);

                ////AccountingPlanTreeNode accountingPlanTreeNode =
                ValueModelTreeNode accountingPlanTreeNode =
                    (new FetchAccountingPlanTreeNodeClass()).OptionFetchAcountingPlanTreeNodeByNameOrLabel(ios,
                                                                                            accountingPlanTreeNodeName,
                                                                                            companyYearAccountingResult.Company,
                                                                                            callerMethodName);

                if (accountingPlanTreeNode == null)
                {
                    accountingPlanTreeNode = (new FetchAccountingPlanTreeNodeClass()).FetchOrCreateAcountingPlanTreeNode(ios,
                                                                                                                 accountingPlanTreeNodeName,
                                                                                                                 accountingPlanTreeNodeLabel,
                                                                                                                 companyYearAccountingResult.Company,
                                                                                                                 callerMethodName);
                    ios.CommitChanges();
                }
                accountingResultByComponent = ios.CreateObject<AccountingResultByComponent>();
                accountingResultByComponent.CompanyYearAccountingResult = companyYearAccountingResult;
                accountingResultByComponent.ActivityCenterNode = activityCenterNode;
                accountingResultByComponent.AccountingPlanTreeNode = accountingPlanTreeNode;
                accountingResultByComponent.IsCompanyLevelObject = false;
                accountingResultByComponent.CompanyName = companyYearAccountingResult.Company.Name;
                accountingResultByComponent.CompanyTreeNode = activityCenterNode.ActivityCenterTreeNode;
            }

            return accountingResultByComponent;
        }
    }
}
