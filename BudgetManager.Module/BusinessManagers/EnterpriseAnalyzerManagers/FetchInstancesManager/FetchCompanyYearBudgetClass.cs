using BudgetManager.Module.BusinessObjects.EnterpriseAnalyzerObjects;
using DevExpress.ExpressApp;
using System.Linq;

namespace BudgetManager.Module.BusinessManagers.EnterpriseAnalyzerManagers
{
    class FetchCompanyYearBudgetClass
    {
        public CompanyYearBudget FetchOrCreateCompanyYearBudget(IObjectSpace ios,
                                                                AugmentedCompany company,
                                                                int year,
                                                                string callerMethodName)
        {
            CompanyYearBudget companyYearBudget = company.CompanyYearBudgets.Where(o => o.Year == year).FirstOrDefault();

            if (companyYearBudget == null)
            {
                companyYearBudget = ios.CreateObject<CompanyYearBudget>();
                companyYearBudget.Company = company;
                companyYearBudget.Year = year;
                companyYearBudget.CompanyName = company.Name;
                companyYearBudget.IsCompanyLevelObject = true;
            }

            return companyYearBudget;
        }

    }
}
