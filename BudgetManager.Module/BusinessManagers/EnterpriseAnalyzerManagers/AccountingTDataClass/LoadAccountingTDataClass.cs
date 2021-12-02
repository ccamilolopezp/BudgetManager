using BudgetManager.Module.BusinessObjects.EnterpriseAnalyzerObjects;
using DevExpress.ExpressApp;
using Finac.nonSQL.Utils;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Text;

namespace BudgetManager.Module.BusinessManagers.EnterpriseAnalyzerManagers
{
    class LoadAccountingTDataClass
    {
        public void LoadAlcaparrosExecutionT(IObjectSpace ios,
                                             AugmentedCompany company,
                                             NPExecutionLoadParameters nPExecutionLoadParameters)
        {

            int periodInMonths = company.AccountingFrame.AccountingPlanTree.PeriodInMonths;

            Tuple<int, int, int, int> yearsMonthsToProcess = (new MonthsToProcessClass()).SelectedYearsAndMonthsToProcess(nPExecutionLoadParameters, periodInMonths);
            int firstYear = yearsMonthsToProcess.Item1;
            int lastYear = yearsMonthsToProcess.Item2;
            int firstYearFirstMonth = yearsMonthsToProcess.Item3;
            int lastYearLastMonth = yearsMonthsToProcess.Item4;

            string periodicityName = "Mensual";

            for (int year = firstYear; year <= lastYear; year++)
            {
                ////CompanyYearBudget companyYearBudget =
                ////FetchCompanyYearBudgetClass.FetchOrCreateCompanyYearBudget(ios, (AugmentedCompany)company, year, "LoadAlcaparrosExecution");

                //////CompanyYearAccountingResultT companyYearAccountingResult =
                //////    FetchCompanyYearAccountingResultClass.FetchOrCreateCompanyYearAccountingResultT(ios,
                //////                                                                            company.Name,
                //////                                                                            year,
                //////                                                                            "LoadAlcaparrosBudget");
                CompanyYearAccountingResult companyYearAccountingResult =
                    (new FetchCompanyYearAccountingResultClass()).FetchOrCreateCompanyYearAccountingResult(ios,
                                                                                            company.Name,
                                                                                            year,
                                                                                            "LoadAlcaparrosBudget");

                Tuple<int, int> initialAndFinalMonthForYear =
                        (new MonthsToProcessClass()).CalculateInitialAndFinalMonthForYear(year, firstYear, lastYear, firstYearFirstMonth, lastYearLastMonth, periodInMonths);
                int initialMonthForYear = initialAndFinalMonthForYear.Item1;
                int finalMonthForYear = initialAndFinalMonthForYear.Item2;

                BudgetPeriod[] aBudgetPeriod =
                    (new FetchBudgetPeriodClass()).FetchBudgetPeriodList(ios, periodicityName, initialMonthForYear, finalMonthForYear, "LoadAlcaparrosExecution").ToArray();
                for (int monthConsecutive = initialMonthForYear; monthConsecutive <= finalMonthForYear; monthConsecutive++)
                {

                    if (nPExecutionLoadParameters.ReplaceExecution)
                    {
                        //Accounting execution results
                        //////IList<AccountingResultByComponentByPeriodT> accountingResultByComponentByPeriodtoDelete =
                        //////FetchAccountingResultByComponentByPeriodClass.FetchAccountingResultByComponentByPeriodListForPeriodT(ios,
                        //////                                                                                                    companyYearAccountingResult,
                        //////                                                                                                    aBudgetPeriod[monthConsecutive - initialMonthForYear],
                        //////                                                                                                    "LoadAlcaparrosExecution");
                        IList<AccountingResultByComponentByPeriod> accountingResultByComponentByPeriodtoDelete =
                        (new FetchAccountingResultByComponentByPeriodClass()).FetchAccountingResultByComponentByPeriodListForPeriod(ios,
                                                                                                                            companyYearAccountingResult,
                                                                                                                            aBudgetPeriod[monthConsecutive - initialMonthForYear],
                                                                                                                            "LoadAlcaparrosExecution");
                        ios.Delete(accountingResultByComponentByPeriodtoDelete);
                        ios.CommitChanges();
                    }

                    string fileNamePrefix = ConfigurationManager.AppSettings["AlcaparrosTerceros"];

                    string fileName = fileNamePrefix + aBudgetPeriod[monthConsecutive - initialMonthForYear].Name + " DE " + year + ".txt";

                    //Accounting execution results
                    LoadAlcaparrosMonthExecutionT(ios, company, year, nPExecutionLoadParameters, fileName, monthConsecutive, companyYearAccountingResult);

                    ios.CommitChanges();
                }

                var observableCompanyYearAccountingResults = company.CompanyYearAccountingResults
                                                            .Where(cyar => cyar.Year == year)
                                                            .ToObservable();

                observableCompanyYearAccountingResults.Subscribe(d =>
                {
                    var observableAccountingResultByComponents = d.AccountingResultByComponents.ToObservable();
                    observableAccountingResultByComponents.Subscribe(e =>
                    {
                        var observableAccountingResultByComponentsByPeriod = e.AccountingResultByComponentByPeriods.ToObservable();
                        observableAccountingResultByComponentsByPeriod.Subscribe(f =>
                        {
                            f.UpdateAccountingResultByComponentByPeriodStatus(true);
                        });
                    });
                    //d.UpdateComponentBudgetStatus(true);
                });

                var observableCompanyYearAccountingResults2 = company.CompanyYearAccountingResults
                                                                .Where(cyar => cyar.Year == year)
                                                                .ToObservable();

                observableCompanyYearAccountingResults2.Subscribe(d =>
                {
                    var observableAccountingResultByComponents = d.AccountingResultByComponents.ToObservable();
                    observableAccountingResultByComponents.Subscribe(e =>
                    {
                        e.UpdateAccountingResultByComponentStatus(true);
                    });
                });

                ////var observableCompanyYearBudgets = company.CompanyYearBudgets
                ////                                                .Where(cyb => cyb.Year == year)
                ////                                                .ToObservable();

                ////observableCompanyYearBudgets.Subscribe(d =>
                ////{
                ////    var observableComponentBudgets = d.ComponentBudgets.ToObservable();
                ////    observableComponentBudgets.Subscribe(e =>
                ////    {
                ////        var observableComponentPeriodBudgets = e.ComponentPeriodBudgets.ToObservable();
                ////        observableComponentPeriodBudgets.Subscribe(f =>
                ////        {
                ////            f.UpdateComponentPeriodBudgetStatus(true);
                ////        });
                ////    });
                ////});

                ////var observableCompanyYearBudgets2 = company.CompanyYearBudgets
                ////                                                .Where(cyb => cyb.Year == year)
                ////                                                .ToObservable();

                ////observableCompanyYearBudgets2.Subscribe(d =>
                ////{
                ////    var observableComponentBudgets = d.ComponentBudgets.ToObservable();
                ////    observableComponentBudgets.Subscribe(e =>
                ////    {
                ////        e.UpdateComponentBudgetStatus(true);
                ////    });
                ////});

                ////var observableCompanyYearBudgets3 = company.CompanyYearBudgets
                ////                                                .Where(cyb => cyb.Year == year)
                ////                                                .ToObservable();

                ////observableCompanyYearBudgets2.Subscribe(d =>
                ////{
                ////    d.UpdateCompanyYearBudgetStatus(true);
                ////});
            }
        }

        public void LoadAlcaparrosMonthExecutionT(IObjectSpace ios,
                                                  Company company,
                                                  int year,////CompanyYearBudget companyYearBudget,
                                                  NPExecutionLoadParameters nPExecutionLoadParameters,
                                                  string fileName,
                                                  int mesConsecutive,
                                                  //////CompanyYearAccountingResultT companyYearAccountingResult)
                                                  CompanyYearAccountingResult companyYearAccountingResult)
        {
            Repository repository = new Repository();



            string[] budgetPeriodNames = BudgetPeriodNamesClass.BudgetPeriodNames();

            ////Company company = companyYearBudget.Company;

            AccountingResultItemType accountingResultItemTypeDebit =
                        (new FetchAccountingResultItemTypeClass()).FetchAccountingResultItemType(ios, "Débito", "LoadAlcaparrosBudgetClass");
            AccountingResultItemType accountingResultItemTypeCredit =
                        (new FetchAccountingResultItemTypeClass()).FetchAccountingResultItemType(ios, "Crédito", "LoadAlcaparrosBudgetClass");

            ///
            ///Esto se pone aquí porque para Alcaparros no hay nada por ActvityCenterNode. Por ahora se asumió "Rectoría".
            CompanyTreeNode companyTreeNode =
                (new FetchCompanyTreeNodeClass()).FetchCompanyTreeNode(ios, "Rectoría", company, "GenerateAccountingResult");


            string periodicityName = "Mensual";
            BudgetPeriod budgetPeriod = FetchBudgetPeriodClass.FetchBudgetPeriod(ios, periodicityName, mesConsecutive, "LoadAlcaparrosBudget");

            //cambió
            DateTime endOfPeriodDate = new DateTime(year, mesConsecutive, DateTime.DaysInMonth(year, mesConsecutive));

            ////Dictionary<string, string> budgetTreeNodeDictionary = ios.GetObjects<BudgetTreeNode>()
            ////                                                      .ToDictionary(btn => btn.Name, btn => btn.Name);

            TextFileCommonClasses.FS_ExecutionFiguresNode[] aExecutionFiguresNode = new TextFileCommonClasses.FS_ExecutionFiguresNode[50000];
            int executionFiguresNodePackCounter = 0;

            Dictionary<string, ValueModelTreeNode> nonPersistedValueModelTreeNodesDictionary = new Dictionary<string, ValueModelTreeNode>();

            string lastAccountCode = "";

            var rofProductRecordPreliminaryCycle = File.ReadLines(fileName, Encoding.UTF8)
                .Select(d => RecordConversion.CreateLiteExecutionFigure(d, nPExecutionLoadParameters.FromThirdPartyInformation)).ToObservable();

            rofProductRecordPreliminaryCycle.Subscribe(d =>
            {
                if (d.Item1 != -1)
                {
                    if (d.Item2.AccountCode != lastAccountCode)
                    {
                        Tuple<bool, ValueModelTreeNode> accountingPlanTreeNodeTuple =
                        (new FetchAccountingPlanTreeNodeClass()).FetchOrCreateAcountingPlanTreeNode(ios,
                                                                                            d.Item2.AccountCode,
                                                                                            d.Item2.AccountLabel,
                                                                                            companyYearAccountingResult.Company,
                                                                                            nonPersistedValueModelTreeNodesDictionary,
                                                                                            "LoadAlcaparrosBudgetClass.LoadAlcaparrosMonthExecution");

                        if (accountingPlanTreeNodeTuple.Item1)
                        {
                            nonPersistedValueModelTreeNodesDictionary.Add(accountingPlanTreeNodeTuple.Item2.Name, accountingPlanTreeNodeTuple.Item2);
                        }

                        lastAccountCode = d.Item2.AccountCode;
                    }

                    if (d.Item2.Credit != 0.0 | d.Item2.Debit != 0.0)
                    {
                        aExecutionFiguresNode[executionFiguresNodePackCounter] = d.Item2;
                        executionFiguresNodePackCounter++;
                    }

                    if (executionFiguresNodePackCounter == 50000)
                    {
                        repository.Insert(aExecutionFiguresNode.Select(o => new MongoPOCOS.CuentaPorTercero
                        {
                            Year = year,
                            Month = mesConsecutive,
                            AccountCode = o.AccountCode,
                            AccountLabel = o.AccountLabel,
                            ClosingBalance = o.ClosingBalance,
                            Credit = o.Credit,
                            Debit = o.Debit,
                            MovementDate = o.MovementDate,
                            Nit = o.Nit,
                            OpeningBalance = o.OpeningBalance
                        }).ToList());

                        aExecutionFiguresNode = new TextFileCommonClasses.FS_ExecutionFiguresNode[50000];
                        executionFiguresNodePackCounter = 0;
                    }
                }
            });
            ios.CommitChanges();

            if (executionFiguresNodePackCounter > 0)
            {
                repository.Insert(aExecutionFiguresNode.Take(executionFiguresNodePackCounter)
                                                        .Select(o => new MongoPOCOS.CuentaPorTercero
                                                        {
                                                            Year = year,
                                                            Month = mesConsecutive,
                                                            AccountCode = o.AccountCode,
                                                            AccountLabel = o.AccountLabel,
                                                            ClosingBalance = o.ClosingBalance,
                                                            Credit = o.Credit,
                                                            Debit = o.Debit,
                                                            MovementDate = o.MovementDate,
                                                            Nit = o.Nit,
                                                            OpeningBalance = o.OpeningBalance
                                                        }).ToList());
            }

            int records = 0;

            lastAccountCode = "";

            var rofProductRecord = File.ReadLines(fileName, Encoding.UTF8)
                .Select(d => RecordConversion.CreateLiteExecutionFigure(d, nPExecutionLoadParameters.FromThirdPartyInformation)).ToObservable();

            rofProductRecord.Subscribe(d =>
            {
                if (d.Item1 != -1)
                {
                    if (d.Item2.AccountCode == lastAccountCode)
                    {
                        var aaa = 0;
                    }
                    else
                    {
                        ProcessAccountingResultRecordT(ios,
                                                  ////companyYearBudget,
                                                  mesConsecutive,
                                                  companyYearAccountingResult,
                                                  company,
                                                  accountingResultItemTypeDebit,
                                                  accountingResultItemTypeCredit,
                                                  periodicityName,
                                                  d,
                                                  companyTreeNode,
                                                  endOfPeriodDate,
                                                  nPExecutionLoadParameters.AssociateToBudget);

                        records++;

                        lastAccountCode = d.Item2.AccountCode;
                    }
                }
            });
        }

        public void ProcessAccountingResultRecordT(IObjectSpace ios,
                                                   ////CompanyYearBudget companyYearBudget,
                                                   int mesConsecutive,
                                                   //////CompanyYearAccountingResultT companyYearAccountingResult,
                                                   CompanyYearAccountingResult companyYearAccountingResult,
                                                   Company company,
                                                   AccountingResultItemType accountingResultItemTypeDebit,
                                                   AccountingResultItemType accountingResultItemTypeCredit,
                                                   //AccountingResultItemType accountingResultItemTypeOpeningBalance,
                                                   //AccountingResultItemType accountingResultItemTypeClosingBalance,
                                                   string periodicityName,
                                                   Tuple<int, TextFileCommonClasses.FS_ExecutionFiguresNode> d,
                                                   CompanyTreeNode companyTreeNode,
                                                   DateTime endOfPeriodDate,
                                                   bool associateToBudget)
        {
            if (d.Item1 != -1)
            {
                //////AccountingResultByComponentByPeriodT accountingResultByComponentByPeriod =
                AccountingResultByComponentByPeriod accountingResultByComponentByPeriod =
                    GenerateAccountingResultT(ios,
                                                company,
                                                companyYearAccountingResult,
                                                companyTreeNode,
                                                d.Item2,
                                                periodicityName,
                                                mesConsecutive,
                                                accountingResultItemTypeDebit,
                                                accountingResultItemTypeCredit,
                                                //accountingResultItemTypeOpeningBalance, 
                                                //accountingResultItemTypeClosingBalance,
                                                endOfPeriodDate);

                ////if (associateToBudget)
                ////{
                ////    ComponentBudget componentBudget =
                ////        FetchComponentBudgetClass.FetchLowestLevelByAccountComponentBudget(ios,
                ////                                                                            companyYearBudget,
                ////                                                                            companyTreeNode.Name,
                ////                                                                            d.Item2.AccountCode,
                ////                                                                            "LoadAlcaparrosBudget");

                ////    if (componentBudget != null)
                ////    {
                ////        ComponentPeriodBudget componentPeriodBudget =
                ////            FetchComponentPeriodBudgetClass.FetchComponentPeriodBudget(componentBudget,
                ////                                                                       periodicityName,
                ////                                                                       mesConsecutive,
                ////                                                                       "LoadAlcaparrosBudget");
                ////        LinkToBudget linkToBudget = ios.CreateObject<LinkToBudget>();
                ////        linkToBudget.AccountingResultByComponentByPeriod = accountingResultByComponentByPeriod;
                ////        linkToBudget.ComponentPeriodBudget = componentPeriodBudget;
                ////        linkToBudget.CompanyName = companyYearBudget.CompanyName;
                ////        linkToBudget.CompanyTreeNode = companyTreeNode;
                ////        linkToBudget.IsLoading = true;
                ////    }
                ////}

            }
        }

        //////public static AccountingResultByComponentByPeriodT GenerateAccountingResultT(IObjectSpace ios,
        public AccountingResultByComponentByPeriod GenerateAccountingResultT(IObjectSpace ios,
                                                                             Company company,
                                                                             //////CompanyYearAccountingResultT accountingResult2015,
                                                                             CompanyYearAccountingResult accountingResult2015,
                                                                             CompanyTreeNode companyTreeNode,
                                                                             TextFileCommonClasses.FS_ExecutionFiguresNode fS_ExecutionFiguresNode,
                                                                             string periodicityName,
                                                                             int periodConsecutive,
                                                                             AccountingResultItemType accountingResultItemTypeDebit,
                                                                             AccountingResultItemType accountingResultItemTypeCredit,
                                                                             //AccountingResultItemType accountingResultItemTypeOpeningBalance,
                                                                             //AccountingResultItemType accountingResultItemTypeClosingBalance, 
                                                                             DateTime endOfPeriodDate)
        {

            //ActivityCenterNode activityCenterNode =
            //    FetchActivityCenterNodeClass.FetchActivityCenterNode(ios, activityCenterNodeName, company, "GenerateAccountingResult");

            //////AccountingResultByComponentT accountingResultByComponent =
            //////    FetchAccountingResultByComponentClass.FetchOrCreateAccountingResultByComponentT(ios,
            //////                                                                                   accountingResult2015,
            //////                                                                                   companyTreeNode.Name,
            //////                                                                                   fS_ExecutionFiguresNode.AccountCode,
            //////                                                                                   fS_ExecutionFiguresNode.AccountLabel,
            //////                                                                                   "GenerateAccountingResult");
            AccountingResultByComponent accountingResultByComponent =
                (new FetchAccountingResultByComponentClass()).FetchOrCreateAccountingResultByComponent(ios,
                                                                                                       accountingResult2015,
                                                                                                       companyTreeNode.Name,
                                                                                                       fS_ExecutionFiguresNode.AccountCode,
                                                                                                       fS_ExecutionFiguresNode.AccountLabel,
                                                                                                       "GenerateAccountingResult");
            //////AccountingResultByComponentByPeriodT accountingResultByComponentByPeriod =
            //////    FetchAccountingResultByComponentByPeriodClass.FetchOrCreateAccountingResultByComponentByPeriodT(ios,
            //////                                                                                                   accountingResultByComponent,
            //////                                                                                                   periodicityName,
            //////                                                                                                   periodConsecutive,
            //////                                                                                                   endOfPeriodDate,
            //////                                                                                                   "GenerateAccountingResult");
            AccountingResultByComponentByPeriod accountingResultByComponentByPeriod =
                (new FetchAccountingResultByComponentByPeriodClass()).FetchOrCreateAccountingResultByComponentByPeriod(ios,
                                                                                                               accountingResultByComponent,
                                                                                                               periodicityName,
                                                                                                               periodConsecutive,
                                                                                                               endOfPeriodDate,
                                                                                                               "GenerateAccountingResult");

            double balanceMultiplier = 1.0;
            if (accountingResultByComponentByPeriod.AccountingResultByComponent.AccountingPlanTreeNode.IsOppositeSign)
                balanceMultiplier = -1.0;

            accountingResultByComponentByPeriod.OpeningBalance = (decimal)(balanceMultiplier * fS_ExecutionFiguresNode.OpeningBalance);
            accountingResultByComponentByPeriod.ClosingBalance = (decimal)(balanceMultiplier * fS_ExecutionFiguresNode.ClosingBalance);

            if (fS_ExecutionFiguresNode.Debit != 0.0)
            {
                GenerateAccountingResultItemT(ios,
                                             accountingResultByComponentByPeriod,
                                             company.Name,
                                             (decimal)fS_ExecutionFiguresNode.Debit,
                                             accountingResultItemTypeDebit);
            }
            if (fS_ExecutionFiguresNode.Credit != 0.0)
            {
                GenerateAccountingResultItemT(ios,
                                             accountingResultByComponentByPeriod,
                                             company.Name,
                                             (decimal)fS_ExecutionFiguresNode.Credit,
                                             accountingResultItemTypeCredit);
            }
            //if (fS_ExecutionFiguresNode.OpeningBalance != 0.0)
            //{
            //    GenerateAccountingResultItem(ios,
            //                                 accountingResultByComponentByPeriod,
            //                                 company.Name,
            //                                 (decimal)fS_ExecutionFiguresNode.OpeningBalance,
            //                                 accountingResultItemTypeOpeningBalance);
            //}
            //if (fS_ExecutionFiguresNode.ClosingBalance != 0.0)
            //{
            //    GenerateAccountingResultItem(ios,
            //                                 accountingResultByComponentByPeriod,
            //                                 company.Name,
            //                                 (decimal)fS_ExecutionFiguresNode.ClosingBalance,
            //                                 accountingResultItemTypeClosingBalance);
            //}

            return accountingResultByComponentByPeriod;
        }

        public void GenerateAccountingResultItemT(IObjectSpace ios,
                                                        //////AccountingResultByComponentByPeriodT accountingResultByComponentByPeriod,
                                                        AccountingResultByComponentByPeriod accountingResultByComponentByPeriod,
                                                        string companyName,
                                                        decimal amount,
                                                        AccountingResultItemType accountingResultItemType)
        {
            //////AccountingResultItemT accountingResultItem =
            //////        ios.CreateObject<AccountingResultItemT>();
            AccountingResultItem accountingResultItem =
                    ios.CreateObject<AccountingResultItem>();

            accountingResultItem.AccountingResultItemType = accountingResultItemType;
            accountingResultItem.Amount = amount;
            accountingResultItem.IsCompanyLevelObject = false;
            accountingResultItem.CompanyName = companyName;
            accountingResultItem.CompanyTreeNode = accountingResultByComponentByPeriod.AccountingResultByComponent.ActivityCenterNode.ActivityCenterTreeNode;
            accountingResultItem.AccountingResultByComponentByPeriod = accountingResultByComponentByPeriod;
            accountingResultItem.IsLoading = true;

            //return accountingResultItem;
        }
    }

}
