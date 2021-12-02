using BudgetManager.Module.BusinessObjects.EnterpriseAnalyzerObjects;
using DevExpress.ExpressApp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BudgetManager.Module.BusinessManagers.EnterpriseAnalyzerManagers
{
    class FetchBudgetPeriodClass
    {
        public static BudgetPeriod FetchBudgetPeriod(IObjectSpace ios,
                                                     string budgetPeriodicityName,
                                                     int consecutive,
                                                     string callerMethodName)
        {
            BudgetPeriod budgetPeriod = ios.GetObjects<BudgetPeriod>()
                                        .Where(ait => ait.BudgetPeriodicity.Name == budgetPeriodicityName & ait.Consecutive == consecutive)
                                        .FirstOrDefault();

            if (budgetPeriod == null)
            {
                throw new Exception("("
                                    + callerMethodName
                                    + ") No existe BudgetPeriod: "
                                    + budgetPeriodicityName
                                    + ", "
                                    + consecutive.ToString());
            }

            return budgetPeriod;
        }

        public BudgetPeriod FetchBudgetPeriod(IObjectSpace ios,
                                              string budgetPeriodicityName,
                                              string name,
                                              string callerMethodName)
        {
            BudgetPeriod budgetPeriod = ios.GetObjectsQuery<BudgetPeriod>()
                                        .Where(ait => ait.BudgetPeriodicity.Name == budgetPeriodicityName & ait.Name == name)
                                        .FirstOrDefault();

            if (budgetPeriod == null)
            {
                throw new Exception("("
                                    + callerMethodName
                                    + ") No existe BudgetPeriod: "

                                    + budgetPeriodicityName
                                    + ", "
                                    + name);
            }

            return budgetPeriod;
        }

        public List<BudgetPeriod> FetchBudgetPeriodList(IObjectSpace ios,
                                                        string budgetPeriodicityName,
                                                        int initialBudgetPeriodConsecutive,
                                                        int finalBudgetPeriodConsecutive,
                                                        string callerMethodName)
        {
            if (!ValidBudgetPeriodConsecutives(initialBudgetPeriodConsecutive, finalBudgetPeriodConsecutive))
            {
                throw new Exception("("
                                    + callerMethodName
                                    + ") Consecutivos inválidos (No existe BudgetPeriod): "
                                    + initialBudgetPeriodConsecutive.ToString()
                                    + ", "
                                    + finalBudgetPeriodConsecutive.ToString());
            }

            BudgetPeriod initialBudgetPeriod =
                FetchBudgetPeriod(ios, budgetPeriodicityName, initialBudgetPeriodConsecutive, callerMethodName);
            BudgetPeriod finalBudgetPeriod =
                FetchBudgetPeriod(ios, budgetPeriodicityName, finalBudgetPeriodConsecutive, callerMethodName);

            List<BudgetPeriod> budgetPeriodList =
                FetchBudgetPeriodList(ios, initialBudgetPeriod, finalBudgetPeriod, callerMethodName);

            return budgetPeriodList;
        }

        public static List<BudgetPeriod> FetchBudgetPeriodList(IObjectSpace ios,
                                                                BudgetPeriod initialBudgetPeriod,
                                                                BudgetPeriod finalBudgetPeriod,
                                                                string callerMethodName)
        {
            List<BudgetPeriod> lBudgetPeriod = new List<BudgetPeriod>();
            if (finalBudgetPeriod == null)
            {
                lBudgetPeriod.Add(initialBudgetPeriod);
            }
            else
            {
                lBudgetPeriod = ios.GetObjects<BudgetPeriod>()
                                .Where(bp => bp.Consecutive >= initialBudgetPeriod.Consecutive
                                             && bp.Consecutive <= finalBudgetPeriod.Consecutive
                                             && bp.BudgetPeriodicity.Name == "Mensual")
                                .OrderBy(bp => bp.Consecutive)
                                .ToList();
            }

            return lBudgetPeriod;
        }

        private bool ValidBudgetPeriodConsecutives(int initialBudgetPeriodConsecutive,
                                                   int finalBudgetPeriodConsecutive)
        {
            bool validBudgetPeriodConsecutives = true;
            if (initialBudgetPeriodConsecutive < 1 || (finalBudgetPeriodConsecutive != 0 && finalBudgetPeriodConsecutive < initialBudgetPeriodConsecutive))
            {
                validBudgetPeriodConsecutives = false;
            }

            return validBudgetPeriodConsecutives;
        }
    }
}
