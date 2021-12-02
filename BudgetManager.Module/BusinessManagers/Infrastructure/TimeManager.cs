using System;
using System.Collections.Generic;

namespace BudgetManager.Module.BusinessManagers
{
    public class TimeManager
    {
        public IEnumerable<DateTime> GetDatesToProcess(DateTime startDate,
                                                       DateTime endDate,
                                                       int step = 1)
        {
            List<DateTime> datesToProcess = new List<DateTime>
            {
                startDate
            };

            bool end = !(startDate.Month == endDate.Month && startDate.Year == endDate.Year);
            DateTime dateToCompare = startDate;
            while (end)
            {
                dateToCompare = dateToCompare.AddMonths(step);
                if (dateToCompare.Month == endDate.Month && dateToCompare.Year == endDate.Year)
                {
                    datesToProcess.Add(endDate);
                    end = false;
                }
                else
                {
                    datesToProcess.Add(dateToCompare);
                }

            }

            return datesToProcess;
        }
    }
}