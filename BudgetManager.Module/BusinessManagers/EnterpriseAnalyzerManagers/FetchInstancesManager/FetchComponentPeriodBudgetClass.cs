using BudgetManager.Module.BusinessObjects.EnterpriseAnalyzerObjects;
using System;
using System.Linq;

namespace BudgetManager.Module.BusinessManagers.EnterpriseAnalyzerManagers
{
    class FetchComponentPeriodBudgetClass
    {
        public ComponentPeriodBudget FetchComponentPeriodBudget(ComponentBudget componentBudget,
                                                                string budgetPeriodicityName,
                                                                int budgetPeriodConsecutive,
                                                                string callerMethodName)
        {
            ComponentPeriodBudget componentPeriodBudget =
                (from cpb in componentBudget.ComponentPeriodBudgets
                 where cpb.BudgetPeriod.Consecutive == budgetPeriodConsecutive
                 select cpb).FirstOrDefault();

            if (componentPeriodBudget == null)
            {
                throw new Exception("("
                                    + callerMethodName
                                    + ") No existe componentPeriodBudget: "
                                    + budgetPeriodicityName
                                    + ", "
                                    + componentBudget.ActivityCenterNode.Name
                                    + ", "
                                    + componentBudget.BudgetTreeNode.Name
                                    + ", "
                                    + componentBudget.CompanyYearBudget.Year.ToString()
                                    + ", "
                                    + budgetPeriodConsecutive.ToString());
            }

            return componentPeriodBudget;
        }
    }
}
