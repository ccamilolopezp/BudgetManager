using BudgetManager.Module.BusinessObjects.EnterpriseAnalyzerObjects;
using System;

namespace BudgetManager.Module.BusinessManagers.EnterpriseAnalyzerManagers
{
    class MonthsToProcessClass
    {
        /// <summary>
        /// Devuelve: - años a procesar 
        ///           - mes del primer año a procesar
        ///           - mes del último año a procesar
        /// </summary>
        /// <param name="nPYearMonthParameters"></param>
        /// <param name="periodInMonths"></param>
        /// <returns></returns>
        public Tuple<int, int, int, int> SelectedYearsAndMonthsToProcess(NPYearMonthParameters nPYearMonthParameters, int periodInMonths)
        {
            int firstYearFirstMonth, lastYearlastMonth;

            if (periodInMonths == 1)
            {
                if (nPYearMonthParameters.FirstMonthFirstYear == 0)
                {
                    firstYearFirstMonth = 1;
                }
                else
                {
                    if (nPYearMonthParameters.FirstMonthFirstYear < 1 | nPYearMonthParameters.FirstMonthFirstYear > 12)
                        throw new Exception("(MonthsToProcessClass.SelectedYearsAndMonthsToProcess) FirstYearFirstMonth inválido: " + nPYearMonthParameters.FirstMonthFirstYear.ToString());

                    firstYearFirstMonth = nPYearMonthParameters.FirstMonthFirstYear;
                }

                if (nPYearMonthParameters.LastMonthLastYear == 0)
                {
                    lastYearlastMonth = 1;
                }
                else
                {
                    if (nPYearMonthParameters.LastMonthLastYear < 1 | nPYearMonthParameters.LastMonthLastYear > 12)
                        throw new Exception("(MonthsToProcessClass.SelectedYearsAndMonthsToProcess) LastYearLastMonth inválido: " + nPYearMonthParameters.LastMonthLastYear.ToString());

                    lastYearlastMonth = nPYearMonthParameters.LastMonthLastYear;
                }
            }
            else
            {
                firstYearFirstMonth = 12;
                lastYearlastMonth = 12;
            }

            int firstYear = nPYearMonthParameters.FirstYear;
            int lastYear = nPYearMonthParameters.LastYear;

            if (firstYear == 0 & lastYear == 0)
                throw new Exception("(MonthsToProcessClass.SelectedYearsAndMonthsToProcess) No se indicaron años");
            if ((firstYear < 1990 & firstYear != 0) | (lastYear < 1990 & lastYear != 0))
                throw new Exception("(MonthsToProcessClass.SelectedYearsAndMonthsToProcess) Algún año inválido (mínimo 1990)");

            if (lastYear == 0)
            {
                lastYear = firstYear;
            }
            if (firstYear == 0)
            {
                firstYear = lastYear;
            }
            firstYear = Math.Min(firstYear, lastYear);

            if ((firstYear == lastYear) & (firstYearFirstMonth > lastYearlastMonth))
                throw new Exception("(MonthsToProcessClass.SelectedYearsAndMonthsToProcess) Primer mes mayor a último mes (no vaplido para un solo año)");

            return new Tuple<int, int, int, int>(firstYear, lastYear, firstYearFirstMonth, lastYearlastMonth);
        }

        public Tuple<int, int> CalculateInitialAndFinalMonthForYear(int year,
                                                                    int firstYear,
                                                                    int lastYear,
                                                                    int firstYearFirstMonth,
                                                                    int lastYearLastMonth,
                                                                    int periodInMonths)
        {
            int initialMonth;
            int finalMonth;

            if (periodInMonths == 12)
            {
                initialMonth = 12;
                finalMonth = 12;
            }
            else
            {
                if (year == firstYear)
                {
                    initialMonth = firstYearFirstMonth;

                    if (firstYear == lastYear)
                    {
                        finalMonth = lastYearLastMonth;
                    }
                    else
                    {
                        finalMonth = 12;
                    }

                }
                else
                {
                    initialMonth = 1;

                    if (year == lastYear)
                    {
                        finalMonth = lastYearLastMonth;
                    }
                    else
                    {
                        finalMonth = 12;
                    }
                }
            }

            return new Tuple<int, int>(initialMonth, finalMonth);
        }
    }
}
