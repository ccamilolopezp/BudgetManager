using BudgetManager.Module.BusinessObjects.EnterpriseAnalyzerObjects;
using DevExpress.ExpressApp;
using System.Linq;

namespace BudgetManager.Module.BusinessManagers.EnterpriseAnalyzerManagers
{
    class GenerateBudgetClass
    {
        public class NPBudgetGenerationRecord
        {
            public string ID { get; set; }
            public string CompanyName { get; set; }
            public int Year { get; set; }
            public string PeriodName { get; set; }
            public ActivityCenterNode ActivityCenterNode { get; set; }
            public BudgetTreeNode BudgetTreeNode { get; set; }
            public decimal Amount { get; set; }
            public string Concept { get; set; }
            public string Project { get; set; }

            #region Constructors

            public NPBudgetGenerationRecord()
            {
            }

            public NPBudgetGenerationRecord(string iD, string companyName, int year, string periodName, ActivityCenterNode activityCenterNode, BudgetTreeNode budgetTreeNode,
                                                decimal amount, string concept, string project)
            {
                ID = iD;
                CompanyName = companyName;
                Year = year;
                PeriodName = periodName;
                ActivityCenterNode = activityCenterNode;
                BudgetTreeNode = budgetTreeNode;
                Amount = amount;
                Concept = Concept;
                Project = project;
            }
            #endregion Constructors
        }

        public class NPBudgetBLC_GenerationRecord
        {
            public string ID { get; set; }
            public string LotID { get; set; }
            public CompanyYearBudget CompanyYearBudget { get; set; }
            public BudgetPeriod BudgetPeriod { get; set; }
            public ActivityCenterNode ActivityCenterNode { get; set; }
            public BudgetTreeNode BudgetTreeNode { get; set; }
            public BLC_GenerationParameters GenerationParameters { get; set; }
            public decimal Amount { get; set; }
            public string Concept { get; set; }
            public string Project { get; set; }

            #region Constructors

            public NPBudgetBLC_GenerationRecord()
            {
            }

            public NPBudgetBLC_GenerationRecord(string iD, string lotID, CompanyYearBudget companyYearBudget, BudgetPeriod budgetPeriod, ActivityCenterNode activityCenterNode,
                                                BudgetTreeNode budgetTreeNode, BLC_GenerationParameters generationParameters, decimal amount, string concept, string project)
            {
                ID = iD;
                LotID = lotID;
                CompanyYearBudget = companyYearBudget;
                BudgetPeriod = budgetPeriod;
                ActivityCenterNode = activityCenterNode;
                BudgetTreeNode = budgetTreeNode;
                GenerationParameters = generationParameters;
                Amount = amount;
                Concept = concept;
                Project = project;
            }

            public NPBudgetBLC_GenerationRecord(string iD, CompanyYearBudget companyYearBudget, BudgetPeriod budgetPeriod, ActivityCenterNode activityCenterNode,
                                                BudgetTreeNode budgetTreeNode, decimal amount, string concept, string project)
            {
                ID = iD;
                LotID = "N/A";
                CompanyYearBudget = companyYearBudget;
                BudgetPeriod = budgetPeriod;
                ActivityCenterNode = activityCenterNode;
                BudgetTreeNode = budgetTreeNode;
                GenerationParameters = null;
                Amount = amount;
                Concept = concept;
                Project = project;
            }

            #endregion Constructors
        }


        public static void GenerateBudget(IObjectSpace ios,
                                         NPBudgetGenerationRecord budgetGenerationRecord,
                                         string lotID,
                                         CompanyYearBudget companyYearBudget,
                                         ComponentBudget componentBudget)
        {
            BudgetPeriod budgetPeriod =
                (new FetchBudgetPeriodClass()).FetchBudgetPeriod(ios, "Mensual", budgetGenerationRecord.PeriodName, "GenerateBudget");

            NPBudgetBLC_GenerationRecord budgetBLC_GenerationRecord = new NPBudgetBLC_GenerationRecord(budgetGenerationRecord.ID,
                                                                                                   companyYearBudget,
                                                                                                   budgetPeriod,
                                                                                                   budgetGenerationRecord.ActivityCenterNode,
                                                                                                   budgetGenerationRecord.BudgetTreeNode,
                                                                                                   budgetGenerationRecord.Amount,
                                                                                                   budgetGenerationRecord.Concept,
                                                                                                   budgetGenerationRecord.Project);

            GenerateBudget(ios, budgetBLC_GenerationRecord, lotID, componentBudget);
        }

        public static void GenerateBudget(IObjectSpace ios, NPBudgetBLC_GenerationRecord budgetGenerationRecord, string lotID,
                                          ComponentBudget componentBudget)
        {
            ComponentPeriodBudgetItem obj = ios.CreateObject<ComponentPeriodBudgetItem>();
            obj.IsLoading = true;

            ComponentPeriodBudget componentPeriodBudget
                = (from snpb in componentBudget.ComponentPeriodBudgets
                   where snpb.BudgetPeriod == budgetGenerationRecord.BudgetPeriod
                   select snpb).FirstOrDefault();

            obj.ComponentPeriodBudget = componentPeriodBudget;

            ////////////////////////////////////////////////////////////////////////////////////////////
            //Quitado mientras se arrgla los CompnayTreeNode (ver FetchOrCreateComponenetBudget)
            obj.CompanyTreeNode = componentPeriodBudget.CompanyTreeNode;
            /////////////////////////////////////////////////////////////////////////////////////////////

            obj.CompanyName = componentPeriodBudget.CompanyName;

            obj.Amount = budgetGenerationRecord.Amount;
            obj.Description = "Descriptionn " + budgetGenerationRecord.Concept;
            obj.LotID = lotID;
            obj.BaseGeneratorID = budgetGenerationRecord.ID;
            obj.Project = budgetGenerationRecord.Project;

            //ios.CommitChanges();
        }
    }
}
