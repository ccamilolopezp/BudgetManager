using BudgetManager.Module.BusinessObjects.EnterpriseAnalyzerObjects;
using DevExpress.ExpressApp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Text;

namespace BudgetManager.Module.BusinessManagers.EnterpriseAnalyzerManagers
{
    class LoadCompanyBudgetRulesClass
    {
        public void LoadCompanyBudgetRules(IObjectSpace ios)
        {
            IList<CompanyBudgetRules> companyBudgetRulestoDelete = ios.GetObjects<CompanyBudgetRules>();
            ios.Delete(companyBudgetRulestoDelete);

            string file = ConfigurationManager.AppSettings["BudgetRules"];
            var budgetRulesRecords = File.ReadLines(file, Encoding.UTF8)
                .Select(d => RecordConversion.CreateLiteTextRecord("CompanyBudgetRules", 4, d)).ToObservable();

            budgetRulesRecords.Subscribe(d =>
            {
                if (d.Item1 == "Util")
                {
                    CompanyBudgetRules obj = ios.CreateObject<CompanyBudgetRules>();
                    obj.IsCompanyLevelObject = true;

                    AugmentedCompany company = (new FetchCompanyClass()).FetchAugmentedCompany(ios, d.Item2[0], false, "LoadCompanyBudgetRulesClass.LoadCompanyBudgetRules");

                    obj.Company = company;
                    obj.CompanyName = company.Name;

                    Tuple<bool, int> initialDayOption = UtilitiesFunctions.CSharpOptionStringToInt(d.Item2[1]);
                    if (!initialDayOption.Item1)
                        throw new Exception("(LoadCompanyBudgetRulesClass.LoadCompanyBudgetRules) InitialDay inválido: " + d.Item2[1]);
                    obj.InitialDay = initialDayOption.Item2;
                    Tuple<bool, int> initialMonthOption = UtilitiesFunctions.CSharpOptionStringToInt(d.Item2[2]);
                    if (!initialMonthOption.Item1)
                        throw new Exception("(LoadCompanyBudgetRulesClass.LoadCompanyBudgetRules) InitialDay inválido: " + d.Item2[2]);
                    obj.InitialMonth = initialMonthOption.Item2;

                    BudgetPeriodicity budgetPeriodicity =
                        (new FetchBudgetPeriodicityClass()).FetchBudgetPeriodicity(ios, d.Item2[3], "LoadCompanyBudgetRulesClass.LoadCompanyBudgetRules");

                    if (budgetPeriodicity == null)
                    {
                        throw new Exception("(LoadDBFromExcelController) No hay BudgetPeriodicity leyendo CompanyBudgetRules: " + d.Item2[3]);
                    }
                    else
                    {
                        obj.BudgetPeriodicity = budgetPeriodicity;
                    }
                }
            });
        }
    }
}
