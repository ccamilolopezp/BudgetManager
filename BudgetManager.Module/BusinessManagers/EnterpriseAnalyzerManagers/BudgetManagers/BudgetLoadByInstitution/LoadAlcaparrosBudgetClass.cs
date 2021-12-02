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
    class LoadAlcaparrosBudgetClass
    {
        public void LoadAlcaparrosBudget(IObjectSpace ios,
                                         Company company,
                                         int year,
                                         int availableMonths,
                                         int firstMonth)
        {
            /////////////////////////////////////////////////////////////////////////
            //Esto podría ser un parámetro. Se uso porque Alcaparros metió una columna
            //más al principio del archivo con el código consolidado.
            //Por ahora la dejamos fija aquí, pero queda como un parámetro para
            //RecordConversion.CreateLiteBudgetReg.
            int columnWithFirstLevelAccountCode = 1;
            /////////////////////////////////////////////////////////////////////////

            //
            //firstMonthIndex: índice del primer mes a procesar (0 a 11)
            //lastMonthIndex: índice del último mes a procesar (0 a 11) y mayor o igual a firstMonthIndex
            int firstMonthIndex = firstMonth - 1;
            int lastMonthIndex = firstMonthIndex + availableMonths - 1;
            if (firstMonthIndex < 0 | firstMonthIndex > 11)
            {
                throw new Exception("(LoadAlcaparrosBudgetClass.LoadAlcaparrosBudget) Primer mes inválido: " + firstMonth.ToString());
            }
            if (lastMonthIndex < firstMonthIndex | lastMonthIndex > 11)
            {
                throw new Exception("(LoadAlcaparrosBudgetClass.LoadAlcaparrosBudget) Ultimo mes inválido:" + lastMonthIndex.ToString()
                                    + " (Meses disponibles: " + availableMonths.ToString() + ", Primer mes: " + firstMonth.ToString() + ")");
            }

            AugmentedCompany augmentedCompany = (AugmentedCompany)ios.GetObjectByKey<Company>(company.CompanyId);

            string fileName = ConfigurationManager.AppSettings["AlcaparrosPresupuesto2016-2017"];
            var rofProductRecord = File.ReadLines(fileName, Encoding.UTF8)
                .Select(d => RecordConversion.CreateLiteBudgetFigure(d, year, availableMonths, columnWithFirstLevelAccountCode)).ToObservable();

            string[] budgetPeriodNames = BudgetPeriodNamesClass.BudgetPeriodNames();

            CompanyYearBudget companyYearBudget = (new FetchCompanyYearBudgetClass()).FetchOrCreateCompanyYearBudget(ios, augmentedCompany, year, "LoadAlcaparrosBudget");

            Dictionary<Tuple<ActivityCenterNode, BudgetTreeNode>, ComponentBudget> componentBudgetDictionary = new Dictionary<Tuple<ActivityCenterNode, BudgetTreeNode>, ComponentBudget>();

            int recordCount = 0;

            rofProductRecord.Subscribe(d =>
            {

                if (d.Item1 != -1) //& recordCount < 1)
                {
                    var a = 0;

                    ActivityCenterNode activityCenterNode =
                        (new FetchActivityCenterNodeClass()).FetchActivityCenterNode(ios,
                                                                             "Rectoría",
                                                                             companyYearBudget.Company,
                                                                             "LoadAlcaparrosBudgetClass.LoadAlcaparrosBudget");

                    BudgetTreeNode budgetTreeNode =
                        (new FetchBudgetTreeNodeClass()).FetchBudgetTreeNodeByNameOrLabel(ios,
                                                                                  d.Item2.AccountCode,
                                                                                  companyYearBudget.Company,
                                                                                  "LoadAlcaparrosBudgetClass.LoadAlcaparrosBudget");

                    ComponentBudget componentBudget;
                    Tuple<ActivityCenterNode, BudgetTreeNode> componentBudgetKey = new Tuple<ActivityCenterNode, BudgetTreeNode>(activityCenterNode, budgetTreeNode);
                    if (componentBudgetDictionary.ContainsKey(componentBudgetKey))
                    {
                        componentBudget = componentBudgetDictionary[componentBudgetKey];
                    }
                    else
                    {
                        componentBudget = (new FetchComponentBudgetClass()).FetchOrCreateComponentBudget(ios,
                                                                               companyYearBudget,
                                                                               activityCenterNode,
                                                                               budgetTreeNode,
                                                                               "LoadAlcaparrosBudgetClass.LoadAlcaparrosBudget");
                        componentBudgetDictionary.Add(componentBudgetKey, componentBudget);
                    }

                    //for (int i = 0; i < availableMonths; i++)
                    //{
                    //    GenerateBudgetClass.NPBudgetGenerationRecord budgetGenerationRecord =
                    //            new GenerateBudgetClass.NPBudgetGenerationRecord("NI-" + i.ToString(),
                    //                                                                  company.Name,
                    //                                                                  year,
                    //                                                                  budgetPeriodNames[i],
                    //                                                                  activityCenterNode,
                    //                                                                  budgetTreeNode,
                    //                                                                  (decimal)d.Item2.BudgetFigures[i],
                    //                                                                  "N/A",
                    //                                                                  "N/A");
                    //    //GenerateBudgetClass.NPTextBudgetGenerationRecord textBudgetGenerationRecord =
                    //    //        new GenerateBudgetClass.NPTextBudgetGenerationRecord("NI-" + i.ToString(),
                    //    //                                                              company.Name,
                    //    //                                                              year,
                    //    //                                                              budgetPeriodNames[i],
                    //    //                                                              "Rectoría",
                    //    //                                                              d.Item2.AccountCode,
                    //    //                                                              (decimal)d.Item2.BudgetFigures[i],
                    //    //                                                              "N/A",
                    //    //                                                              "N/A");
                    //    GenerateBudgetClass.GenerateBudget(ios, budgetGenerationRecord, "NI", companyYearBudget, componentBudget);


                    //}

                    recordCount++;

                }
            });

            ios.CommitChanges();

            recordCount = 0;

            var rofProductRecord2 = File.ReadLines(fileName, Encoding.UTF8)
                .Select(d => RecordConversion.CreateLiteBudgetFigure(d, year, availableMonths, columnWithFirstLevelAccountCode)).ToObservable();

            rofProductRecord2.Subscribe(d =>
            {

                if (d.Item1 != -1) //& recordCount < 1)
                {
                    var a = 0;

                    ActivityCenterNode activityCenterNode =
                        (new FetchActivityCenterNodeClass()).FetchActivityCenterNode(ios,
                                                                                     "Rectoría",
                                                                                     companyYearBudget.Company,
                                                                                     "LoadAlcaparrosBudgetClass.LoadAlcaparrosBudget");

                    BudgetTreeNode budgetTreeNode =
                        (new FetchBudgetTreeNodeClass()).FetchBudgetTreeNodeByNameOrLabel(ios,
                                                                                          d.Item2.AccountCode,
                                                                                          companyYearBudget.Company,
                                                                                          "LoadAlcaparrosBudgetClass.LoadAlcaparrosBudget");

                    ComponentBudget componentBudget;
                    var tuple = Tuple.Create("", 1);
                    Tuple<ActivityCenterNode, BudgetTreeNode> componentBudgetKey = Tuple.Create(activityCenterNode, budgetTreeNode);
                    if (componentBudgetDictionary.ContainsKey(componentBudgetKey))
                    {
                        componentBudget = componentBudgetDictionary[componentBudgetKey];
                    }
                    else
                    {
                        componentBudget = (new FetchComponentBudgetClass()).FetchOrCreateComponentBudget(ios,
                                                                                                         companyYearBudget,
                                                                                                         activityCenterNode,
                                                                                                         budgetTreeNode,
                                                                                                         "LoadAlcaparrosBudgetClass.LoadAlcaparrosBudget");
                        componentBudgetDictionary.Add(componentBudgetKey, componentBudget);
                    }

                    for (int i = firstMonthIndex; i <= lastMonthIndex; i++)
                    {
                        GenerateBudgetClass.NPBudgetGenerationRecord budgetGenerationRecord =
                                new GenerateBudgetClass.NPBudgetGenerationRecord("NI-" + i.ToString(),
                                                                                 company.Name,
                                                                                 year,
                                                                                 budgetPeriodNames[i],
                                                                                 activityCenterNode,
                                                                                 budgetTreeNode,
                                                                                 (decimal)d.Item2.BudgetFigures[i - firstMonthIndex],
                                                                                 "N/A",
                                                                                 "N/A");

                        //GenerateBudgetClass.NPTextBudgetGenerationRecord textBudgetGenerationRecord =
                        //        new GenerateBudgetClass.NPTextBudgetGenerationRecord("NI-" + i.ToString(),
                        //                                                              company.Name,
                        //                                                              year,
                        //                                                              budgetPeriodNames[i],
                        //                                                              "Rectoría",
                        //                                                              d.Item2.AccountCode,
                        //                                                              (decimal)d.Item2.BudgetFigures[i],
                        //                                                              "N/A",
                        //                                                              "N/A");
                        GenerateBudgetClass.GenerateBudget(ios, budgetGenerationRecord, "NI", companyYearBudget, componentBudget);


                    }

                    recordCount++;

                }
            });

            ios.CommitChanges();

            var observableComponentBudgets = companyYearBudget.ComponentBudgets.ToObservable();

            observableComponentBudgets.Subscribe(d =>
            {
                var observableComponentPeriodBudgets = d.ComponentPeriodBudgets.ToObservable();
                observableComponentPeriodBudgets.Subscribe(e =>
                {
                    e.UpdateComponentPeriodBudgetStatus(true);
                });
                //d.UpdateComponentBudgetStatus(true);
            });

            var observableComponentBudgets2 = companyYearBudget.ComponentBudgets.ToObservable();

            observableComponentBudgets2.Subscribe(d =>
            {
                d.UpdateComponentBudgetStatus(true);
            });

            companyYearBudget.UpdateCompanyYearBudgetStatus(true);

            //ios.GetObjects<ComponentPeriodBudget>()
            //                                      .Where(cpb => cpb.ComponentBudget.CompanyName == company.Name)
            //                                      .ToObservable();

            //observableComponentPeriodBudget.Subscribe(d =>
            //{
            //    d.UpdateComponentPeriodBudgetStatus(true);
            //});

        }

        public void LoadAlcaparrosExecution(IObjectSpace ios,
                                            AugmentedCompany company,
                                            NPExecutionLoadParameters nPExecutionLoadParameters)
        {

            //Tuple<int, int, bool, bool> monthsToProcessTuple = MonthsToProcessClass.SelectedMonthsToProcess(nPExecutionLoadParameters);
            //int initialMonthConsecutive = monthsToProcessTuple.Item1;
            //int finalMonthConsecutive = monthsToProcessTuple.Item2;
            //bool initialMonthFound = monthsToProcessTuple.Item3;
            //bool finalMonthFound = monthsToProcessTuple.Item4;

            int periodInMonths = company.AccountingFrame.AccountingPlanTree.PeriodInMonths;

            Tuple<int, int, int, int> yearsMonthsToProcess = (new MonthsToProcessClass()).SelectedYearsAndMonthsToProcess(nPExecutionLoadParameters, periodInMonths);
            int firstYear = yearsMonthsToProcess.Item1;
            int lastYear = yearsMonthsToProcess.Item2;
            int firstYearFirstMonth = yearsMonthsToProcess.Item3;
            int lastYearLastMonth = yearsMonthsToProcess.Item4;

            //IList<AccountingResultItem> accountingResultItemtoDelete = ios.GetObjects<AccountingResultItem>();
            //ios.Delete(accountingResultItemtoDelete);
            //IList<AccountingResultByComponentByPeriod> accountingResultByComponentByPeriodtoDelete = ios.GetObjects<AccountingResultByComponentByPeriod>();
            //ios.Delete(accountingResultByComponentByPeriodtoDelete);
            //IList<AccountingResultByComponent> accountingResultByComponenttoDelete = ios.GetObjects<AccountingResultByComponent>();
            //ios.Delete(accountingResultByComponenttoDelete);
            //IList<CompanyYearAccountingResult> companyYearAccountingResulttoDelete = ios.GetObjects<CompanyYearAccountingResult>();
            //ios.Delete(companyYearAccountingResulttoDelete);
            //IList<LinkToBudget> linkToBudgettoDelete = ios.GetObjects<LinkToBudget>();
            //ios.Delete(linkToBudgettoDelete);
            //ios.CommitChanges();

            string periodicityName = "Mensual";

            for (int year = firstYear; year <= lastYear; year++)
            {
                CompanyYearBudget companyYearBudget =
                (new FetchCompanyYearBudgetClass()).FetchOrCreateCompanyYearBudget(ios, (AugmentedCompany)company, year, "LoadAlcaparrosExecution");

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
                        IList<AccountingResultByComponentByPeriod> accountingResultByComponentByPeriodtoDelete =
                        (new FetchAccountingResultByComponentByPeriodClass()).FetchAccountingResultByComponentByPeriodListForPeriod(ios,
                                                                                                                            companyYearAccountingResult,
                                                                                                                            aBudgetPeriod[monthConsecutive - initialMonthForYear],
                                                                                                                            "LoadAlcaparrosExecution");
                        ios.Delete(accountingResultByComponentByPeriodtoDelete);
                        ios.CommitChanges();
                    }

                    string fileNamePrefix = ConfigurationManager.AppSettings["AlcaparrosEjecutado"];

                    string fileName = fileNamePrefix + aBudgetPeriod[monthConsecutive - initialMonthForYear].Name + " " + year + ".txt";

                    if (nPExecutionLoadParameters.FromThirdPartyInformation)
                    {
                        fileNamePrefix = ConfigurationManager.AppSettings["AlcaparrosTerceros"];

                        fileName = fileNamePrefix + aBudgetPeriod[monthConsecutive - initialMonthForYear].Name + " DE " + year + ".txt";

                        //Accounting execution results
                        //LoadAccountingTDataClass.LoadAlcaparrosMonthExecutionT(ios, company, year, nPExecutionLoadParameters, fileName, monthConsecutive, companyYearAccountingResult);

                    }
                    //Accounting execution results
                    LoadAlcaparrosMonthExecution(ios, companyYearBudget, nPExecutionLoadParameters, fileName, monthConsecutive, companyYearAccountingResult);

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

                var observableCompanyYearBudgets = company.CompanyYearBudgets
                                                                .Where(cyb => cyb.Year == year)
                                                                .ToObservable();

                observableCompanyYearBudgets.Subscribe(d =>
                {
                    var observableComponentBudgets = d.ComponentBudgets.ToObservable();
                    observableComponentBudgets.Subscribe(e =>
                    {
                        var observableComponentPeriodBudgets = e.ComponentPeriodBudgets.ToObservable();
                        observableComponentPeriodBudgets.Subscribe(f =>
                        {
                            f.UpdateComponentPeriodBudgetStatus(true);
                        });
                    });
                });

                var observableCompanyYearBudgets2 = company.CompanyYearBudgets
                                                                .Where(cyb => cyb.Year == year)
                                                                .ToObservable();

                observableCompanyYearBudgets2.Subscribe(d =>
                {
                    var observableComponentBudgets = d.ComponentBudgets.ToObservable();
                    observableComponentBudgets.Subscribe(e =>
                    {
                        e.UpdateComponentBudgetStatus(true);
                    });
                });

                var observableCompanyYearBudgets3 = company.CompanyYearBudgets
                                                                .Where(cyb => cyb.Year == year)
                                                                .ToObservable();

                observableCompanyYearBudgets2.Subscribe(d =>
                {
                    d.UpdateCompanyYearBudgetStatus(true);
                });
            }
        }

        public void LoadAlcaparrosMonthExecution(IObjectSpace ios,
                                                 CompanyYearBudget companyYearBudget,
                                                 NPExecutionLoadParameters nPExecutionLoadParameters,
                                                 string fileName,
                                                 int mesConsecutive,
                                                 CompanyYearAccountingResult companyYearAccountingResult)
        {

            //
            //Estas variables se necesitan cuando se usa información de terceros.
            Repository repository = new Repository();
            TextFileCommonClasses.FS_ExecutionFiguresNode[] aExecutionFiguresNode
                = new TextFileCommonClasses.FS_ExecutionFiguresNode[50000]; ;
            int executionFiguresNodePackCounter = 0;

            string[] budgetPeriodNames = BudgetPeriodNamesClass.BudgetPeriodNames();

            Company company = companyYearBudget.Company;
            int year = companyYearBudget.Year;

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

            DateTime endOfPeriodDate = new DateTime(companyYearBudget.Year, mesConsecutive, DateTime.DaysInMonth(companyYearBudget.Year, mesConsecutive));

            Dictionary<string, string> budgetTreeNodeDictionary = ios.GetObjects<BudgetTreeNode>()
                                                                  .ToDictionary(btn => btn.Name, btn => btn.Name);

            Dictionary<string, ValueModelTreeNode> nonPersistedValueModelTreeNodesDictionary = new Dictionary<string, ValueModelTreeNode>();

            //
            //Esta variable se necesita porque cuando se carga por terceros hay varios registros para la misma cuenta.
            //El primero es el total, que es el que nos interesa. Los demás solo interesan para almacenar la
            //información por terceros.
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

                    if (nPExecutionLoadParameters.FromThirdPartyInformation)
                    {
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
                }
            });
            ios.CommitChanges();

            if (nPExecutionLoadParameters.FromThirdPartyInformation & executionFiguresNodePackCounter > 0)
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
                //if(nPExecutionLoadParameters.FromThirdPartyInformation)
                //{
                //    LoadAccountingTDataClass.ProcessAccountingResultRecordT(ios,
                //                                  ////companyYearBudget,
                //                                  mesConsecutive,
                //                                  companyYearAccountingResult,
                //                                  company,
                //                                  accountingResultItemTypeDebit,
                //                                  accountingResultItemTypeCredit,
                //                                  periodicityName,
                //                                  d,
                //                                  companyTreeNode,
                //                                  endOfPeriodDate,
                //                                  nPExecutionLoadParameters.AssociateToBudget);
                //}
                //else
                //{
                if (d.Item1 == -1)
                {
                    var xcc = 0;
                }
                else
                {
                    var xcc = 0;
                }

                if (d.Item2.AccountCode == "-PC-1" | d.Item2.AccountCode == "-PC-2")
                {
                    var xc = 0;
                }
                if (d.Item2.AccountCode != lastAccountCode)
                {
                    ProcessAccountingResultRecord(ios,

                                                  companyYearBudget,
                                                  mesConsecutive,
                                                  companyYearAccountingResult,
                                                  company,
                                                  accountingResultItemTypeDebit,
                                                  accountingResultItemTypeCredit,
                                                  //accountingResultItemTypeOpeningBalance, 
                                                  //accountingResultItemTypeClosingBalance,
                                                  periodicityName,
                                                  d,
                                                  companyTreeNode,
                                                  endOfPeriodDate,
                                                  nPExecutionLoadParameters.AssociateToBudget,
                                                  nPExecutionLoadParameters.FromThirdPartyInformation);

                    //}

                    records++;
                }
                lastAccountCode = d.Item2.AccountCode;
            });
        }

        private void ProcessAccountingResultRecord(IObjectSpace ios,
                                                  CompanyYearBudget companyYearBudget,
                                                  int mesConsecutive,
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
                                                 bool associateToBudget,
                                                 bool fromThirdPartyInformation)
        {
            if (d.Item1 != -1)
            {
                AccountingResultByComponentByPeriod accountingResultByComponentByPeriod;
                if (fromThirdPartyInformation)
                {
                    accountingResultByComponentByPeriod =
                    (new LoadAccountingTDataClass()).GenerateAccountingResultT(ios,
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
                }
                else
                {
                    accountingResultByComponentByPeriod =
                    GenerateAccountingResult(ios,
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
                }


                if (associateToBudget)
                {
                    ComponentBudget componentBudget =
                        (new FetchComponentBudgetClass()).FetchLowestLevelByAccountComponentBudget(ios,
                                                                                            companyYearBudget,
                                                                                            companyTreeNode.Name,
                                                                                            d.Item2.AccountCode,
                                                                                            "LoadAlcaparrosBudget");

                    if (componentBudget != null)
                    {
                        ComponentPeriodBudget componentPeriodBudget =
                            (new FetchComponentPeriodBudgetClass()).FetchComponentPeriodBudget(componentBudget,
                                                                                       periodicityName,
                                                                                       mesConsecutive,
                                                                                       "LoadAlcaparrosBudget");
                        LinkToBudget linkToBudget = ios.CreateObject<LinkToBudget>();
                        linkToBudget.AccountingResultByComponentByPeriod = accountingResultByComponentByPeriod;
                        linkToBudget.ComponentPeriodBudget = componentPeriodBudget;
                        linkToBudget.CompanyName = companyYearBudget.CompanyName;
                        linkToBudget.CompanyTreeNode = companyTreeNode;
                        linkToBudget.IsLoading = true;
                    }
                }

                //ios.CommitChanges();
            }
        }
        public AccountingResultByComponentByPeriod GenerateAccountingResult(IObjectSpace ios,
                                                                            Company company,
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

            AccountingResultByComponent accountingResultByComponent =
                (new FetchAccountingResultByComponentClass()).FetchOrCreateAccountingResultByComponent(ios,
                                                                                               accountingResult2015,
                                                                                               companyTreeNode.Name,
                                                                                               fS_ExecutionFiguresNode.AccountCode,
                                                                                               fS_ExecutionFiguresNode.AccountLabel,
                                                                                               "GenerateAccountingResult");
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
                GenerateAccountingResultItem(ios,
                                             accountingResultByComponentByPeriod,
                                             company.Name,
                                             (decimal)fS_ExecutionFiguresNode.Debit,
                                             accountingResultItemTypeDebit);
            }
            if (fS_ExecutionFiguresNode.Credit != 0.0)
            {
                GenerateAccountingResultItem(ios,
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
        public void GenerateAccountingResultItem(IObjectSpace ios,
                                                 AccountingResultByComponentByPeriod accountingResultByComponentByPeriod,
                                                 string companyName,
                                                 decimal amount,
                                                 AccountingResultItemType accountingResultItemType)
        {
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
