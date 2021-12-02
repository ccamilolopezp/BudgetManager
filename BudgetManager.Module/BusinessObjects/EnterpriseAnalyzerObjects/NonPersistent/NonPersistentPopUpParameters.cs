using DevExpress.ExpressApp.DC;

namespace BudgetManager.Module.BusinessObjects.EnterpriseAnalyzerObjects
{

    [DomainComponent]
    public class NPBudgetLoadParameters
    {
        public bool EraseExistingBudget { get; set; }
        public int Year { get; set; }
        public int FirstMonth { get; set; }
        public int AvailableMonths { get; set; }
    }

    [DomainComponent]
    public class NPYearMonthParameters : NPMonthParameters
    {
        public int FirstYear { get; set; }
        public int LastYear { get; set; }
    }

    [DomainComponent]
    public class NPMonthParameters
    {
        public int FirstMonthFirstYear { get; set; }
        public int LastMonthLastYear { get; set; }

    }

    [DomainComponent]
    public class NPExecutionLoadParameters : NPYearMonthParameters
    {
        public bool ReplaceExecution { get; set; }
        public bool AssociateToBudget { get; set; }
        public bool FromThirdPartyInformation { get; set; }
    }

    class BudgetPeriodNamesClass
    {
        public static string[] BudgetPeriodNames()
        {
            string[] budgetPeriodNames = new string[12];
            budgetPeriodNames[0] = "01-Enero";
            budgetPeriodNames[1] = "02-Febrero";
            budgetPeriodNames[2] = "03-Marzo";
            budgetPeriodNames[3] = "04-Abril";
            budgetPeriodNames[4] = "05-Mayo";
            budgetPeriodNames[5] = "06-Junio";
            budgetPeriodNames[6] = "07-Julio";
            budgetPeriodNames[7] = "08-Agosto";
            budgetPeriodNames[8] = "09-Septiembre";
            budgetPeriodNames[9] = "10-Octubre";
            budgetPeriodNames[10] = "11-Noviembre";
            budgetPeriodNames[11] = "12-Diciembre";

            return budgetPeriodNames;
        }

    }
}
