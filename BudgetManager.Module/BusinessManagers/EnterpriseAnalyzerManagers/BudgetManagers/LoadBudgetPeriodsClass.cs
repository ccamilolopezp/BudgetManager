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
    class LoadBudgetPeriodsClass
    {
        public void LoadBudgetPeriods(IObjectSpace ios)
        {
            IList<BudgetPeriodicity> budgetPeriodicitytoDelete = ios.GetObjects<BudgetPeriodicity>();
            ios.Delete(budgetPeriodicitytoDelete);

            string file = ConfigurationManager.AppSettings["BudgetPeriodicity"];
            var periodicityRecords = File.ReadLines(file, Encoding.UTF8)
                .Select(d => RecordConversion.CreateLiteTextRecord("BudgetPeriodicity", 2, d)).ToObservable();

            Dictionary<string, BudgetPeriodicity> budgetPeriodicityDictionary = new Dictionary<string, BudgetPeriodicity>();

            periodicityRecords.Subscribe(d =>
            {
                if (d.Item1 == "Util")
                {
                    BudgetPeriodicity obj = ios.CreateObject<BudgetPeriodicity>();
                    obj.Name = d.Item2[0];
                    Tuple<bool, int> periodInMonthsOption = UtilitiesFunctions.CSharpOptionStringToInt(d.Item2[1]);
                    if (!periodInMonthsOption.Item1)
                        throw new Exception("(LoadBudgetPeriodClass) PeriodInMonths inválido: " + d.Item2[1]);
                    obj.PeriodInMonths = periodInMonthsOption.Item2;

                    budgetPeriodicityDictionary.Add(obj.Name, obj);
                }
            });

            file = ConfigurationManager.AppSettings["BudgetPeriod"];
            var periodRecords = File.ReadLines(file, Encoding.UTF8)
                .Select(d => RecordConversion.CreateLiteTextRecord("Budget Periods", 5, d)).ToObservable();

            periodRecords.Subscribe(d =>
            {
                if (d.Item1 == "Util")
                {
                    BudgetPeriod obj = ios.CreateObject<BudgetPeriod>();

                    BudgetPeriodicity budgetPeriodicity;
                    if (!budgetPeriodicityDictionary.TryGetValue(d.Item2[0], out budgetPeriodicity)) // Returns true.
                    {
                        throw new Exception("(LoadBudgetPeriodClass) PeriodInMonths inválido: " + d.Item2[0]);
                    }

                    obj.BudgetPeriodicity = budgetPeriodicity;
                    Tuple<bool, int> consecutiveOption = UtilitiesFunctions.CSharpOptionStringToInt(d.Item2[1]);
                    if (!consecutiveOption.Item1)
                        throw new Exception("(LoadBudgetPeriodClass) Consecutive inválido: " + d.Item2[1]);
                    obj.Consecutive = consecutiveOption.Item2;

                    string stringConsecutive = d.Item2[1];
                    if (1 == stringConsecutive.Length)
                    {
                        stringConsecutive = "0" + stringConsecutive;
                    }

                    obj.Name = stringConsecutive + "-" + d.Item2[2];
                    Tuple<bool, int> initialMonthOption = UtilitiesFunctions.CSharpOptionStringToInt(d.Item2[3]);
                    if (!initialMonthOption.Item1)
                        throw new Exception("(LoadBudgetPeriodClass) InitialMonth inválido: " + d.Item2[3]);
                    obj.InitialMonth = initialMonthOption.Item2;
                    Tuple<bool, int> finalMonthOption = UtilitiesFunctions.CSharpOptionStringToInt(d.Item2[4]);
                    if (!finalMonthOption.Item1)
                        throw new Exception("(LoadBudgetPeriodClass) finalMonth inválido: " + d.Item2[4]);
                    obj.FinalMonth = finalMonthOption.Item2;
                }
            });
        }

    }
}
