using BudgetManager.Module.BusinessObjects.EnterpriseAnalyzerObjects;
using DevExpress.ExpressApp;
using System;
using System.Linq;

namespace BudgetManager.Module.BusinessManagers.EnterpriseAnalyzerManagers
{
    class FetchBudgetPeriodicityClass
    {
        public BudgetPeriodicity FetchBudgetPeriodicity(IObjectSpace ios,
                                                        string budgetPeriodicityName,
                                                        string callerMethodName)
        {
            BudgetPeriodicity budgetPeriodicity = ios.GetObjects<BudgetPeriodicity>()
                                        .Where(ait => ait.Name == budgetPeriodicityName)
                                        .FirstOrDefault();

            if (budgetPeriodicity == null)
            {
                throw new Exception("("
                                    + callerMethodName
                                    + ") No existe BudgetPeriodicity: "
                                    + budgetPeriodicityName);
            }

            return budgetPeriodicity;
        }
    }
}
