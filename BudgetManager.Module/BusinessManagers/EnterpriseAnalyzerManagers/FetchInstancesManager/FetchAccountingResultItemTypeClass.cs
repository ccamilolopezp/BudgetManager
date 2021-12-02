using BudgetManager.Module.BusinessObjects.EnterpriseAnalyzerObjects;
using DevExpress.ExpressApp;
using System;
using System.Linq;

namespace BudgetManager.Module.BusinessManagers.EnterpriseAnalyzerManagers
{
    class FetchAccountingResultItemTypeClass
    {
        public AccountingResultItemType FetchAccountingResultItemType(IObjectSpace ios,
                                                                      string accountingResultItemTypeName,
                                                                      string callerMethodName)
        {
            AccountingResultItemType accountingResultItemType = ios.GetObjects<AccountingResultItemType>()
                                                                .Where(ap => ap.Name == accountingResultItemTypeName)
                                                                .FirstOrDefault();

            if (accountingResultItemType == null)
            {
                throw new Exception("("
                                    + callerMethodName
                                    + ") No existe AccountingResultItemType: "
                                    + accountingResultItemTypeName);
            }

            return accountingResultItemType;
        }
    }
}
