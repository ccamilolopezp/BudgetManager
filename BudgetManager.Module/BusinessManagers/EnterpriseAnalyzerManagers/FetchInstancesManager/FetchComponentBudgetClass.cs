using BudgetManager.Module.BusinessObjects.EnterpriseAnalyzerObjects;
using DevExpress.ExpressApp;
using System.Linq;
using System.Reactive.Linq;

namespace BudgetManager.Module.BusinessManagers.EnterpriseAnalyzerManagers
{
    class FetchComponentBudgetClass
    {
        public ComponentBudget FetchOrCreateComponentBudget(IObjectSpace ios,
                                                                     CompanyYearBudget companyYearBudget,
                                                                     ActivityCenterNode activityCenterNode,
                                                                     BudgetTreeNode budgetTreeNode,
                                                                     string callerMethodName)
        {
            //ComponentBudget componentBudgetA = ios.GetObjects<ComponentBudget>()
            //                                  .Where(cb => cb.CompanyYearBudget.CompanyYearBudgetId == companyYearBudget.CompanyYearBudgetId
            //                                                & cb.ActivityCenterNode.ActivityCenterNodeId == activityCenterNode.ActivityCenterNodeId
            //                                                & cb.BudgetTreeNode.BudgetTreeNodeId == budgetTreeNode.BudgetTreeNodeId)
            //                                  .FirstOrDefault();

            ComponentBudget componentBudget = null;
            //por ahora se quitó porque por la forma de la hoja los componentBuget se crean una sola vez.
            //Cuando se manejen actualizaciones hay que volver a ponerlos.
            //ios.GetObjectsQuery<ComponentBudget>()
            //                                  .Where(cb => cb.CompanyYearBudget.CompanyYearBudgetId == companyYearBudget.CompanyYearBudgetId
            //                                                & cb.ActivityCenterNode.ActivityCenterTreeNode.Name == activityCenterNode.ActivityCenterTreeNode.Name
            //                                                & cb.BudgetTreeNode.Name == budgetTreeNode.Name)
            //                                  .FirstOrDefault();

            if (componentBudget == null)
            {
                componentBudget = ios.CreateObject<ComponentBudget>();
                componentBudget.IsLoading = true;
                componentBudget.ActivityCenterNode = activityCenterNode;
                componentBudget.CompanyYearBudget = companyYearBudget;
                componentBudget.BudgetTreeNode = budgetTreeNode;
                componentBudget.CompanyName = companyYearBudget.Company.Name;

                ///////////////////////////////////////////////////////////////////////////////
                //Esto se quitó emporalmente hasta que se arregle lo de los CompanyTreeNodeAncestors, que
                //quedan mal para seniority > 0
                CompanyTreeNode companyTreeNode =
                    (new ActivityCenterFetchAttributesClass()).FetchActivityCenterStructureTreeNode(activityCenterNode,
                                                                                                    "LoadBudgetTreesController");
                componentBudget.CompanyTreeNode = companyTreeNode;
                ///////////////////////////////////////////////////////////////////////////////


                componentBudget.Description = budgetTreeNode.Label;

                foreach (BudgetPeriod budgetPeriod in companyYearBudget.Company.CompanyBudgetRules.BudgetPeriodicity.BudgetPeriods)
                {
                    ComponentPeriodBudget componentPeriodBudget = ios.CreateObject<ComponentPeriodBudget>();
                    componentPeriodBudget.IsLoading = true;
                    componentPeriodBudget.ComponentBudget = componentBudget;
                    componentPeriodBudget.BudgetPeriod = budgetPeriod;

                    ///////////////////////////////////////////////////////////////////////////////
                    //Esto también se quitó emporalmente hasta que se arregle lo de los CompmnayTreeNodeAncestors, que
                    //quedan mal para seniority > 0
                    componentPeriodBudget.CompanyTreeNode = companyTreeNode;
                    ///////////////////////////////////////////////////////////////////////////////

                    componentPeriodBudget.CompanyName = companyYearBudget.Company.Name;
                }

                //ios.CommitChanges();
            }

            return componentBudget;
        }

        public ComponentBudget FetchLowestLevelByAccountComponentBudget(IObjectSpace ios,
                                                                        CompanyYearBudget companyYearBudget,
                                                                        string activityCenterNodeName,
                                                                        string accountingPlanTreeNodeName,
                                                                        string callerMethodName)
        {
            bool componentBudgetFound = false;
            ComponentBudget lowestLevelByAccountComponentBudget = null;

            ////AccountingPlanTreeNode accountingPlanTreeNode =
            ValueModelTreeNode accountingPlanTreeNode =
                    (new FetchAccountingPlanTreeNodeClass()).FetchAcountingPlanTreeNodeByNameOrLabel(ios,
                                                                                            accountingPlanTreeNodeName,
                                                                                            companyYearBudget.Company,
                                                                                            callerMethodName);

            ////AccountingPlanTreeNodeAncestor[] aAccountingPlanTreeNodeAncestor =
            ValueModelTreeNodeAncestor[] aAccountingPlanTreeNodeAncestor =
                (from aptn in accountingPlanTreeNode.ValueModelTreeNodeAncestors
                 orderby aptn.Seniority
                 select aptn).ToArray();

            int i = 0;
            while (!componentBudgetFound & i < aAccountingPlanTreeNodeAncestor.Length)
            {
                string aAccountingPlanTreeNodeAncestoriName = aAccountingPlanTreeNodeAncestor[i].Name;

                lowestLevelByAccountComponentBudget = ios.GetObjectsQuery<ComponentBudget>()
                                                        .Where(cb => cb.CompanyYearBudget.CompanyYearBudgetId == companyYearBudget.CompanyYearBudgetId
                                                                     && cb.ActivityCenterNode.ActivityCenterTreeNode.Name == activityCenterNodeName
                                                                     && cb.BudgetTreeNode.Name == aAccountingPlanTreeNodeAncestoriName)
                                                                     .FirstOrDefault();

                if (lowestLevelByAccountComponentBudget != null)
                {
                    componentBudgetFound = true;
                }

                i++;
            }

            ////////////////////////////////////
            //var observableAccountingPlanTreeNodeAncestor = accountingPlanTreeNode.AccountingPlanTreeNodeAncestors
            //                                               .OrderBy(apltna => apltna.Seniority)
            //                                               .ToObservable();


            //observableAccountingPlanTreeNodeAncestor.Subscribe(d =>
            //{

            //});

            //int i = 0;
            //while (!componentBudgetFound & i < aAccountingPlanTreeNodeAncestor.Length)
            //{
            //    lowestLevelByAccountComponentBudget = ios.GetObjects<ComponentBudget>()
            //                                            .Where(cb => cb.CompanyYearBudget == companyYearBudget
            //                                                         && cb.ActivityCenterNode.Name == activityCenterNodeName
            //                                                         && cb.BudgetTreeNode.Name == aAccountingPlanTreeNodeAncestor[i].Name)
            //                                                         .FirstOrDefault();

            //    if (lowestLevelByAccountComponentBudget != null)
            //    {
            //        componentBudgetFound = true;
            //    }

            //    i++;
            //}

            ////////////////////////////////////

            return lowestLevelByAccountComponentBudget;
        }

    }
}
