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
    class LoadBudgetTreeClass
    {
        public void LoadAlcaparrosBudgetTreeContable(IObjectSpace ios, Company company)
        {
            /////////////////////////////////////////////////////////////////////////
            //Estos podrían ser parámetros. Se usaron porque Alcaparros metió una columna
            //más al principio del archivo con el código consolidado.
            //Por ahora la dejamos fija aquí, pero queda como un parámetro para
            //RecordConversion.CreateLiteBudgetReg.
            int columnWithFirstLevelAccountCode = 1;
            int accountLabelColumn = 7;
            /////////////////////////////////////////////////////////////////////////

            Company companyThisSession = ios.GetObjectByKey<Company>(company.CompanyId);

            AugmentedCompany augmentedCompany = (AugmentedCompany)companyThisSession;

            if (augmentedCompany.BudgetTree != null)
            {
                ios.Delete(augmentedCompany.BudgetTree);
            }

            string fileName = ConfigurationManager.AppSettings["AlcaparrosPresupuesto2016-2017"];
            var rofProductRecord = File.ReadLines(fileName, Encoding.UTF8)
                .Select(d => RecordConversion.CreateLiteAccount(d, columnWithFirstLevelAccountCode, accountLabelColumn)).ToObservable();

            //Esto es el cargue de cuentas original que se tenía
            //string path = @"C:\ApplicationFiles\Tesoreria\Alcaparros\1. Enero 2014.txt";
            //var rofProductRecord = new RegexObservableFile(new Regex(".{10,1000}", RegexOptions.Compiled | RegexOptions.Multiline),
            //                                 path)
            //          .Select(d => RecordConversion.CreateLiteRegAccountingBudget(d.Value));

            BudgetTree budgetTree = ios.CreateObject<BudgetTree>();
            budgetTree.Company = augmentedCompany;

            budgetTree.CompanyName = company.Name;
            budgetTree.IsCompanyLevelObject = true;

            Dictionary<string, string> budgetNodesDictionary = new Dictionary<string, string>();
            List<Tuple<string, string>> repeatedAccounts = new List<Tuple<string, string>>();

            string headNode = "BudgetNode";

            Stack<BudgetTreeNode> nodeStack = new Stack<BudgetTreeNode>();
            BudgetTreeNode budgetTreeHeadNode = CreateBudgetTreeNode(ios, budgetTree, headNode, true, false, null);

            nodeStack.Push(budgetTreeHeadNode);

            BudgetTreeNode popedBudgetTreeNode = null;

            int createdNodes = 1;

            rofProductRecord.Subscribe(d =>
            {

                if (d.Item1 != -1)
                {
                    //
                    //inicailmente revisa si la cuenta ya existe. Si existe, la ignora.
                    string foundAccount = "";
                    bool accountFound = budgetNodesDictionary.TryGetValue(d.Item2.AccountCode, out foundAccount);
                    if (!accountFound)
                    {
                        budgetNodesDictionary.Add(d.Item2.AccountCode, d.Item2.AccountLabel);

                        while (Regex.Matches(d.Item2.AccountCode, "-").Count <= Regex.Matches(nodeStack.Peek().Name, "-").Count)
                        {
                            popedBudgetTreeNode = nodeStack.Pop();
                        }

                        BudgetTreeNode treeNodeAncestor = nodeStack.Peek();
                        BudgetTreeNode treeNode =
                            CreateBudgetTreeNode(ios, budgetTree, d.Item2.AccountCode, d.Item2.AccountLabel, false, false, treeNodeAncestor);
                        createdNodes += 1;
                        nodeStack.Push(treeNode);
                    }
                    else
                    {
                        repeatedAccounts.Add(new Tuple<string, string>(d.Item2.AccountCode, d.Item2.AccountLabel));
                    }
                }
            });

            ios.CommitChanges();

            foreach (BudgetTreeNode budgetTreeNode in budgetTree.BudgetTreeNodes)
            {
                if (budgetTreeNode.Children.Count() == 0)
                {
                    budgetTreeNode.IsEndNode = true;
                }
            }

            var a = 0;
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
        public BudgetTreeNode CreateBudgetTreeNode(IObjectSpace ios,
                                                                   BudgetTree budgetTree,
                                                                   string nodeName,
                                                                   bool isHeadNode,
                                                                   bool isEndNode,
                                                                   BudgetTreeNode budgetTreeNodeAncestor)
        {
            return CreateBudgetTreeNode(ios,
                                         budgetTree,
                                         nodeName,
                                         nodeName,
                                         isHeadNode,
                                         isEndNode,
                                         budgetTreeNodeAncestor);

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
        public BudgetTreeNode CreateBudgetTreeNode(IObjectSpace ios,
                                                                   BudgetTree budgetTree,
                                                                   string nodeName,
                                                                   string nodeLabel,
                                                                   bool isHeadNode,
                                                                   bool isEndNode,
                                                                   BudgetTreeNode budgetTreeNodeAncestor)
        {
            BudgetTreeNode Hobj = ios.CreateObject<BudgetTreeNode>();
            Hobj.BudgetTree = budgetTree;
            Hobj.Name = nodeName;
            Hobj.Label = nodeLabel;
            //Hobj.InvestmentUnitType = investmentUnitType;
            Hobj.IsHeadNode = isHeadNode;
            Hobj.IsEndNode = isEndNode;
            Hobj.Parent = budgetTreeNodeAncestor;
            Hobj.CreateAncestorsOnSaving = true;


            //CreateBudgetTreeNodeAncestors(ios, Hobj);

            return Hobj;
        }

        //public static void CreateBudgetTreeNodeAncestors(IObjectSpace ios,
        //                                                           BudgetTreeNode budgetTreeNode)
        //{

        //    ios.Delete(budgetTreeNode.BudgetTreeNodeAncestors);

        //    BudgetTreeNode currentNode = budgetTreeNode;

        //    bool currentNodeHasParent = true;
        //    int seniority = 0;

        //    BudgetTreeNodeAncestor treeNodeAncestor = ios.CreateObject<BudgetTreeNodeAncestor>();
        //    treeNodeAncestor.BudgetTreeNode = budgetTreeNode;
        //    treeNodeAncestor.Name = budgetTreeNode.Name;
        //    treeNodeAncestor.Seniority = seniority;

        //    do
        //    {
        //        seniority += 1;

        //        if (!currentNode.IsHeadNode)
        //        {
        //            treeNodeAncestor = ios.CreateObject<BudgetTreeNodeAncestor>();
        //            treeNodeAncestor.BudgetTreeNode = budgetTreeNode;

        //            if (currentNode.Parent != null)
        //            {
        //                treeNodeAncestor.Name = currentNode.Parent.Name;
        //                treeNodeAncestor.Seniority = seniority;

        //                budgetTreeNode.BudgetTreeNodeAncestors.Add(treeNodeAncestor);
        //                currentNode = (BudgetTreeNode)currentNode.Parent;
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
