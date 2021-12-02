using BudgetManager.Module.BusinessObjects.EnterpriseAnalyzerObjects;
using DevExpress.ExpressApp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace BudgetManager.Module.BusinessManagers.EnterpriseAnalyzerManagers
{
    class LoadAlcaparrosLedgerAccountsPlantClass
    {
        public void LoadAlcaparrosAccountingPlanTree(IObjectSpace ios, Company company)
        {
            Company companyThisSession = ios.GetObjectByKey<Company>(company.CompanyId);

            AugmentedCompany augmentedCompany = (AugmentedCompany)companyThisSession;

            //
            //elimina el AccouningFrame si existe
            if (augmentedCompany.AccountingFrame != null)
            {
                if (augmentedCompany.AccountingFrame.AccountingPlanTree != null)
                {
                    ios.Delete(augmentedCompany.AccountingFrame.AccountingPlanTree);
                }
                ios.Delete(augmentedCompany.AccountingFrame);
            }

            string fileName = ConfigurationManager.AppSettings["AlcaparrosEjecutadoEnero2015"];
            var rofProductRecord = File.ReadLines(fileName, Encoding.UTF8)
                .Select(d => RecordConversion.CreateLiteAccountingPlanRecord(d)).ToObservable();


            //Esto es el cargue de cuentas original que se tenía
            //string path = @"C:\ApplicationFiles\Tesoreria\Alcaparros\1. Enero 2014.txt";
            //var rofProductRecord = new RegexObservableFile(new Regex(".{10,1000}", RegexOptions.Compiled | RegexOptions.Multiline),
            //                                 path)
            //          .Select(d => RecordConversion.CreateLiteRegAccountingBudget(d.Value));

            //
            //ya no es necesario porque se crea el -PC en RecordConversion
            string accountPrefix = "";

            AccountingFrame accountingFrame = ios.CreateObject<AccountingFrame>();
            ValueModelTree accountingPlanTree = ios.CreateObject<ValueModelTree>();
            accountingPlanTree.Name = "PlanDeCuentas";
            accountingPlanTree.AccountPrefix = accountPrefix;
            accountingPlanTree.PeriodInMonths = 1;

            accountingFrame.Company = augmentedCompany;
            accountingFrame.AccountingPlanTree = accountingPlanTree;

            accountingFrame.CompanyName = augmentedCompany.Name;
            accountingFrame.IsCompanyLevelObject = true;

            string headNode = "-PC";

            ////Stack<AccountingPlanTreeNode> nodeStack = new Stack<AccountingPlanTreeNode>();
            Stack<ValueModelTreeNode> nodeStack = new Stack<ValueModelTreeNode>();
            ////AccountingPlanTreeNode accountingPlanTreeHeadNode = CreateAccountingPlanTreeNode(ios, accountingPlanTree, headNode, true, false, null);
            ValueModelTreeNode accountingPlanTreeHeadNode = CreateAccountingPlanTreeNode(ios, accountingPlanTree, headNode, true, false, null, accountPrefix);

            nodeStack.Push(accountingPlanTreeHeadNode);

            ////AccountingPlanTreeNode popedAccountingPlanTreeNode = null;
            ValueModelTreeNode popedAccountingPlanTreeNode = null;

            int createdNodes = 1;

            rofProductRecord.Subscribe(d =>
            {

                if (d.Item1 != -1)
                {
                    //while (Regex.Matches(d.Item2.AccountCode, "-").Count <= Regex.Matches(nodeStack.Peek().ID, "-").Count)
                    while (Regex.Matches((accountPrefix + d.Item2.AccountCode), "-").Count <= Regex.Matches(nodeStack.Peek().Name, "-").Count)
                    {
                        popedAccountingPlanTreeNode = nodeStack.Pop();
                        //if(firstPop)
                        //{

                        //}
                    }

                    ////AccountingPlanTreeNode treeNodeAncestor = nodeStack.Peek();
                    ValueModelTreeNode treeNodeAncestor = nodeStack.Peek();
                    ////AccountingPlanTreeNode treeNode =
                    ValueModelTreeNode treeNode =
                        //CreateBudgetTreeNode(ios, budgetTree, d.Item2.AccountName, d.Item2.AccountCode, false, false, treeNodeAncestor);
                        CreateAccountingPlanTreeNode(ios, accountingPlanTree, d.Item2.AccountCode, d.Item2.AccountLabel, false, false, treeNodeAncestor, accountPrefix);
                    createdNodes += 1;
                    nodeStack.Push(treeNode);


                    //}
                    /////
                    /////Cualquier registro que tenga un valor antes de la columna 4. Es decir, 
                    /////trae nodos anteriores al nodo del producto.
                    //if (d.Item1 != 3)
                    //{
                    //    int numberToPop = nodeStack.Count - d.Item1;
                    //    for (int i = 1; i <= numberToPop; i++)
                    //    {
                    //        var a = nodeStack.Pop();
                    //    }
                    //    for (int i = d.Item1; i < 3; i++)
                    //    {
                    //        BudgetTreeNode treeNodeAncestor = nodeStack.Peek();
                    //        BudgetTreeNode treeNode =
                    //            CreateBudgetTreeNode(ios, budgetTree, d.Item2[i - 1], false, false, treeNodeAncestor);
                    //        nodeStack.Push(treeNode);

                    //    }
                    //}

                    /////
                    /////Se pregunta si el Item2[3] que es el contenido de la columna 4 no es
                    /////vacío porque pueden venir registros vacíos
                    //if (d.Item2[2] != "")
                    //{
                    //    BudgetTreeNode budgetTreeNodeAncestor = nodeStack.Peek();
                    //    BudgetTreeNode budgetTreeNode =
                    //            CreateBudgetTreeNode(ios, budgetTree, d.Item2[2], false, true, budgetTreeNodeAncestor);

                    //    //if (nPProductTreeLoadingParameters.CreateCashMovementoriginatorItemType)
                    //    //{

                    //    //}
                    //}
                }
            });

            ////foreach (AccountingPlanTreeNode accountingPlanTreeNode in accountingPlanTree.AccountingPlanTreeNodes)
            foreach (ValueModelTreeNode accountingPlanTreeNode in accountingPlanTree.ValueModelTreeNodes)
            {
                if (accountingPlanTreeNode.Children.Count() == 0)
                {
                    accountingPlanTreeNode.IsEndNode = true;
                }
            }
        }

        /// <summary>
        /// Usa el nodeName para Name y ID 
        /// </summary>
        /// <param name="ios"></param>
        /// <param name="budgetTree"></param>
        /// <param name="nodeName"></param>
        /// <param name="isHeadNode"></param>
        /// <param name="isEndNode"></param>
        /// <param name="budgetTreeNodeAncestor"></param>
        /// <returns></returns>
        ////Cambio a ValueModelTree
        ////public static AccountingPlanTreeNode CreateAccountingPlanTreeNode(IObjectSpace ios,
        public  ValueModelTreeNode CreateAccountingPlanTreeNode(IObjectSpace ios,
                                                                   ////AccountingPlanTree accountingPlanTree,
                                                                   ValueModelTree accountingPlanTree,
                                                                   string nodeName,
                                                                   bool isHeadNode,
                                                                   bool isEndNode,
                                                                   ValueModelTreeNode budgetTreeNodeAncestor,
                                                                   string accountPrefix) //AccountingPlanTreeNode budgetTreeNodeAncestor)
        {
            return CreateAccountingPlanTreeNode(ios,
                                         accountingPlanTree,
                                         nodeName,
                                         nodeName,
                                         isHeadNode,
                                         isEndNode,
                                         budgetTreeNodeAncestor,
                                         accountPrefix);

        }

        /// <summary>
        /// Crea un BudgetTreeNode
        /// </summary>
        /// <param name="ios"></param>
        /// <param name="budgetTree"></param>
        /// <param name="nodeID"></param>
        /// <param name="nodeName"></param>
        /// <param name="isHeadNode"></param>
        /// <param name="isEndNode"></param>
        /// <param name="budgetTreeNodeAncestor"></param>
        /// <returns></returns>
        public ValueModelTreeNode CreateAccountingPlanTreeNode(IObjectSpace ios,
                                                                   ////AccountingPlanTree accountingPlanTree,
                                                                   ValueModelTree accountingPlanTree,
                                                                   string nodeName,
                                                                   string nodeLabel,
                                                                   bool isHeadNode,
                                                                   bool isEndNode,
                                                                   ValueModelTreeNode accountingPlanTreeNodeAncestor,
                                                                   string accountPrefix)
        {
            ////AccountingPlanTreeNode Hobj = ios.CreateObject<AccountingPlanTreeNode>();
            ValueModelTreeNode Hobj = ios.CreateObject<ValueModelTreeNode>();
            ////Hobj.AccountingPlanTree = accountingPlanTree;
            Hobj.ValueModelTree = accountingPlanTree;
            Hobj.Label = nodeLabel;
            Hobj.Name = accountPrefix + nodeName;
            //Hobj.InvestmentUnitType = investmentUnitType;
            Hobj.IsHeadNode = isHeadNode;
            Hobj.IsEndNode = isEndNode;
            Hobj.Parent = accountingPlanTreeNodeAncestor;

            if (Regex.Match(nodeName, "^-PC-2").Success | Regex.Match(nodeName, "^-PC-3").Success | Regex.Match(nodeName, "^-PC-4").Success)
            {
                Hobj.IsOppositeSign = true;
            }

            Hobj.CreateAncestorsOnSaving = true;

            //CreateAccountingPlanTreeNodeAncestors(ios, Hobj);

            return Hobj;
        }

        //public static void CreateAccountingPlanTreeNodeAncestors(IObjectSpace ios,
        //                                                           AccountingPlanTreeNode accountingPlanTreeNode)
        //{

        //    ios.Delete(accountingPlanTreeNode.AccountingPlanTreeNodeAncestors);

        //    AccountingPlanTreeNode currentNode = accountingPlanTreeNode;

        //    bool currentNodeHasParent = true;
        //    int seniority = 0;

        //    AccountingPlanTreeNodeAncestor treeNodeAncestor = ios.CreateObject<AccountingPlanTreeNodeAncestor>();
        //    treeNodeAncestor.AccountingPlanTreeNode = accountingPlanTreeNode;
        //    treeNodeAncestor.Name = accountingPlanTreeNode.Name;
        //    treeNodeAncestor.Seniority = seniority;

        //    do
        //    {
        //        seniority += 1;

        //        if (!currentNode.IsHeadNode)
        //        {
        //            treeNodeAncestor = ios.CreateObject<AccountingPlanTreeNodeAncestor>();
        //            treeNodeAncestor.AccountingPlanTreeNode = accountingPlanTreeNode;

        //            if (currentNode.Parent != null)
        //            {
        //                treeNodeAncestor.Name = currentNode.Parent.Name;
        //                treeNodeAncestor.Seniority = seniority;

        //                accountingPlanTreeNode.AccountingPlanTreeNodeAncestors.Add(treeNodeAncestor);
        //                currentNode = (AccountingPlanTreeNode)currentNode.Parent;
        //            }
        //            else
        //            {
        //                currentNodeHasParent = false;
        //            }

        //        }

        //    } while (!currentNode.IsHeadNode && currentNodeHasParent);
        //}
    }
}
