using BudgetManager.Module.BusinessObjects.EnterpriseAnalyzerObjects;
using DevExpress.ExpressApp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BudgetManager.Module.BusinessManagers.EnterpriseAnalyzerManagers
{
    class FetchAccountingResultByComponentByPeriodClass
    {
        public AccountingResultByComponentByPeriod FetchOrCreateAccountingResultByComponentByPeriod(IObjectSpace ios,
                                                                                                    AccountingResultByComponent accountingResultByComponent,
                                                                                                    string periodicityName,
                                                                                                    int periodConsecutive,
                                                                                                    DateTime endOfPeriodDate,
                                                                                                    string callerMethodName)
        {
            AccountingResultByComponentByPeriod accountingResultByComponentByPeriod =
                ios.GetObjectsQuery<AccountingResultByComponentByPeriod>()
                .Where(arbcbp => arbcbp.AccountingResultByComponent.AccountingPlanTreeNode.Name == accountingResultByComponent.AccountingPlanTreeNode.Name
                                     && arbcbp.AccountingResultByComponent.AccountingResultByComponentId == accountingResultByComponent.AccountingResultByComponentId
                                     && arbcbp.AccountingPeriod.BudgetPeriodicity.Name == periodicityName
                                     && arbcbp.AccountingPeriod.Consecutive == periodConsecutive)
                .FirstOrDefault();

            if (accountingResultByComponentByPeriod == null)
            {
                BudgetPeriod accountingPeriod = FetchBudgetPeriodClass.FetchBudgetPeriod(ios, periodicityName, periodConsecutive, callerMethodName);

                accountingResultByComponentByPeriod = ios.CreateObject<AccountingResultByComponentByPeriod>();
                accountingResultByComponentByPeriod.AccountingResultByComponent = accountingResultByComponent;
                accountingResultByComponentByPeriod.AccountingPeriod = accountingPeriod;
                accountingResultByComponentByPeriod.IsCompanyLevelObject = false;
                accountingResultByComponentByPeriod.CompanyName = accountingResultByComponent.CompanyName;
                accountingResultByComponentByPeriod.CompanyTreeNode = accountingResultByComponent.CompanyTreeNode;
                accountingResultByComponentByPeriod.IsLoading = true;
                accountingResultByComponentByPeriod.EnOfPeriodDate = endOfPeriodDate;
                accountingResultByComponentByPeriod.AccountCode = accountingResultByComponent.AccountingPlanTreeNode.Name;
            }

            return accountingResultByComponentByPeriod;
        }

        public List<AccountingResultByComponentByPeriod> FetchAccountingResultByComponentByPeriodListForPeriod(IObjectSpace ios,
                                                                                                               CompanyYearAccountingResult companyYearAccountingResult,
                                                                                                               BudgetPeriod budgetPeriod,
                                                                                                               string callerMethodName)
        {
            List<AccountingResultByComponentByPeriod> accountingResultByComponentByPeriodListForPeriod =
                ios.GetObjects<AccountingResultByComponentByPeriod>()
                .Where(arbcbp => arbcbp.AccountingResultByComponent.CompanyYearAccountingResult == companyYearAccountingResult
                         & arbcbp.AccountingPeriod.Name == budgetPeriod.Name)
                .ToList();

            return accountingResultByComponentByPeriodListForPeriod;
        }

    }
}
