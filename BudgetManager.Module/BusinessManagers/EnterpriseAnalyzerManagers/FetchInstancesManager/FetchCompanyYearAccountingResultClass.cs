using BudgetManager.Module.BusinessObjects.EnterpriseAnalyzerObjects;
using DevExpress.ExpressApp;
using System.Linq;

namespace BudgetManager.Module.BusinessManagers.EnterpriseAnalyzerManagers
{
    class FetchCompanyYearAccountingResultClass
    {
        public CompanyYearAccountingResult FetchOrCreateCompanyYearAccountingResult(IObjectSpace ios,
                                                                                    string companyName,
                                                                                    int year,
                                                                                    string callerMethodName)
        {
            CompanyYearAccountingResult companyYearAccountingResult =
                ios.GetObjects<CompanyYearAccountingResult>()
                                                  .Where(cyar => cyar.Company.Name == companyName
                                                                & cyar.Year == year)
                                                  .FirstOrDefault();

            if (companyYearAccountingResult == null)
            {
                CompanyYearAccountingResult obj = ios.CreateObject<CompanyYearAccountingResult>();

                AugmentedCompany companyThisSession =
                    (new FetchCompanyClass()).FetchAugmentedCompany(ios, companyName, false, "LoadBLC_Budget-FetchOrCreateCompanyYearAccountingResult");

                obj.Company = companyThisSession;
                obj.Year = year;
                obj.CompanyName = companyName;
                obj.IsCompanyLevelObject = true;

                return obj;
            }
            else
            {
                return companyYearAccountingResult;
            }
        }

    }
}
