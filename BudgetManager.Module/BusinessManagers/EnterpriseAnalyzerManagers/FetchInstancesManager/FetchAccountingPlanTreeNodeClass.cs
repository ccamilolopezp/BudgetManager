using BudgetManager.Module.BusinessObjects.EnterpriseAnalyzerObjects;
using DevExpress.ExpressApp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BudgetManager.Module.BusinessManagers.EnterpriseAnalyzerManagers
{
    class FetchAccountingPlanTreeNodeClass
    {
        public ValueModelTreeNode FetchAcountingPlanTreeNodeByNameOrLabel(IObjectSpace ios,
                                                                          string accountingPlanTreeNodeNameOrLabel,
                                                                          AugmentedCompany company,
                                                                          string callerMethodName)
        {
            ////ValueModelTreeNode accountingPlanTreeNode = FetchAcountingPlanTreeNodeByNameOrLabel(ios,
            ////                                                     accountingPlanTreeNodeNameOrLabel,
            ////                                                     company.Name,
            ////                                                     callerMethodName);

            ValueModelTreeNode accountingPlanTreeNode = company.AccountingFrame.AccountingPlanTree.ValueModelTreeNodes
                                                        .Where(btn => btn.Label == accountingPlanTreeNodeNameOrLabel
                                                                      || btn.Name == accountingPlanTreeNodeNameOrLabel)
                                                        .FirstOrDefault();

            if (accountingPlanTreeNode == null)
            {
                throw new Exception("("
                                    + callerMethodName
                                    + ") No existe AccountingPlanTreeNode: "
                                    + accountingPlanTreeNodeNameOrLabel
                                    + " , "
                                    + company.Name);
            }
            return accountingPlanTreeNode;
        }

        public ValueModelTreeNode FetchAcountingPlanTreeNodeByName(IObjectSpace ios,
                                                                   string accountingPlanTreeNodeName,
                                                                   AugmentedCompany company,
                                                                   Dictionary<string, ValueModelTreeNode> nonPersistedValueModelTreeNodesDictionary,
                                                                   string callerMethodName)
        {
            ////ValueModelTreeNode accountingPlanTreeNode = FetchAcountingPlanTreeNodeByNameOrLabel(ios,
            ////                                                     accountingPlanTreeNodeNameOrLabel,
            ////                                                     company.Name,
            ////                                                     callerMethodName);

            ValueModelTreeNode accountingPlanTreeNode = company.AccountingFrame.AccountingPlanTree.ValueModelTreeNodes
                                                        .Where(btn => btn.Name == accountingPlanTreeNodeName)
                                                        .FirstOrDefault();

            if (accountingPlanTreeNode == null)
            {
                if (!nonPersistedValueModelTreeNodesDictionary.TryGetValue(accountingPlanTreeNodeName, out accountingPlanTreeNode)) // Returns true.
                {
                    throw new Exception("("
                                    + callerMethodName
                                    + ") No existe AccountingPlanTreeNode: "
                                    + accountingPlanTreeNodeName
                                    + " , "
                                    + company.Name);
                }
            }
            return accountingPlanTreeNode;
        }

        public Tuple<bool, ValueModelTreeNode> FetchOrCreateAcountingPlanTreeNode(IObjectSpace ios,
                                                                                  string accountingPlanTreeNodeName,
                                                                                  string accountingPlanTreeNodeLabel,
                                                                                  AugmentedCompany company,
                                                                                  Dictionary<string, ValueModelTreeNode> nonPersistedValueModelTreeNodesDictionary,
                                                                                  string callerMethodName)
        {
            bool isCreated = false;

            ValueModelTreeNode accountingPlanTreeNode = company.AccountingFrame.AccountingPlanTree.ValueModelTreeNodes
                                                        .Where(btn => btn.Label == accountingPlanTreeNodeLabel
                                                                      & btn.Name == accountingPlanTreeNodeName)
                                                        .FirstOrDefault();

            if (accountingPlanTreeNode == null)
            {
                string accountingPlanParentTreeNodeName = "";
                int separatorLastIndex = accountingPlanTreeNodeName.LastIndexOf('-');
                if (separatorLastIndex == -1 | separatorLastIndex == 0)
                {
                    throw new Exception("No se puede generar código del ancestro para la cuenta que se quiere crear: " + accountingPlanTreeNodeName);
                }
                else
                {
                    accountingPlanParentTreeNodeName = accountingPlanTreeNodeName.Substring(0, separatorLastIndex);
                    ValueModelTreeNode accountingPlanParentTreeNode =
                        FetchAcountingPlanTreeNodeByName(ios,
                                                                accountingPlanParentTreeNodeName,
                                                                company,
                                                                nonPersistedValueModelTreeNodesDictionary,
                                                                callerMethodName);
                    accountingPlanTreeNode = ios.CreateObject<ValueModelTreeNode>();
                    accountingPlanTreeNode.Label = accountingPlanTreeNodeLabel;
                    accountingPlanTreeNode.Name = accountingPlanTreeNodeName;
                    accountingPlanTreeNode.ValueModelTree = accountingPlanParentTreeNode.ValueModelTree;
                    accountingPlanTreeNode.Parent = accountingPlanParentTreeNode;
                    accountingPlanTreeNode.CreateAncestorsOnSaving = true;
                    //accountingPlanTreeNode.Save();
                }

                isCreated = true;

                //var accountingPlanParentTreeNodeName = DataLoadFunctions.AccountCodelevel("333"); // .AncestorAccountCode(accountingPlanTreeNodeName);
                //let AncestorAccountCode ( code : string) =
                //
                //    let separatorLastIndex = code.LastIndexOf('-')
                //    
                //    let ancestorAccountCode = match separatorLastIndex with
                //                              | -1 
                //                              | 0 -> ""
                //                              | _ ->  code.Substring(0,separatorLastIndex)
                //
                //    ancestorAccountCode


            }

            return new Tuple<bool, ValueModelTreeNode>(isCreated, accountingPlanTreeNode);
        }

        public ValueModelTreeNode OptionFetchAcountingPlanTreeNodeByNameOrLabel(IObjectSpace ios,
                                                                                string accountingPlanTreeNodeNameOrLabel,
                                                                                AugmentedCompany company,
                                                                                string callerMethodName)
        {
            ////ValueModelTreeNode accountingPlanTreeNode = OptionFetchAcountingPlanTreeNodeByNameOrLabel(ios,
            ////                                                                                         accountingPlanTreeNodeNameOrLabel,
            ////                                                                                         company.Name,
            ////                                                                                         callerMethodName);

            ValueModelTreeNode accountingPlanTreeNode = company.AccountingFrame.AccountingPlanTree.ValueModelTreeNodes
                                                        .Where(btn => btn.Label == accountingPlanTreeNodeNameOrLabel
                                                                      || btn.Name == accountingPlanTreeNodeNameOrLabel)
                                                        .FirstOrDefault();

            return accountingPlanTreeNode;
        }

        public ValueModelTreeNode FetchOrCreateAcountingPlanTreeNode(IObjectSpace ios,
                                                                     string accountingPlanTreeNodeName,
                                                                     string accountingPlanTreeNodeLabel,
                                                                     AugmentedCompany company,
                                                                     string callerMethodName)
        {
            ValueModelTreeNode accountingPlanTreeNode = company.AccountingFrame.AccountingPlanTree.ValueModelTreeNodes
                                                        .Where(btn => btn.Label == accountingPlanTreeNodeLabel
                                                                      & btn.Name == accountingPlanTreeNodeName)
                                                        .FirstOrDefault();

            if (accountingPlanTreeNode == null)
            {
                string accountingPlanParentTreeNodeName = "";
                int separatorLastIndex = accountingPlanTreeNodeName.LastIndexOf('-');
                if (separatorLastIndex == -1 | separatorLastIndex == 0)
                {
                    throw new Exception("No se puede generar código del ancestro para la cuenta que se quiere crear: " + accountingPlanTreeNodeName);
                }
                else
                {
                    accountingPlanParentTreeNodeName = accountingPlanTreeNodeName.Substring(0, separatorLastIndex);
                    ValueModelTreeNode accountingPlanParentTreeNode =
                        FetchAcountingPlanTreeNodeByNameOrLabel(ios,
                                                                accountingPlanParentTreeNodeName,
                                                                company,
                                                                callerMethodName);
                    accountingPlanTreeNode = ios.CreateObject<ValueModelTreeNode>();
                    accountingPlanTreeNode.Label = accountingPlanTreeNodeLabel;
                    accountingPlanTreeNode.Name = accountingPlanTreeNodeName;
                    accountingPlanTreeNode.ValueModelTree = accountingPlanParentTreeNode.ValueModelTree;
                    accountingPlanTreeNode.Parent = accountingPlanParentTreeNode;
                    accountingPlanTreeNode.CreateAncestorsOnSaving = true;
                    //accountingPlanTreeNode.Save();
                }

                //var accountingPlanParentTreeNodeName = DataLoadFunctions.AccountCodelevel("333"); // .AncestorAccountCode(accountingPlanTreeNodeName);
                //let AncestorAccountCode ( code : string) =
                //
                //    let separatorLastIndex = code.LastIndexOf('-')
                //    
                //    let ancestorAccountCode = match separatorLastIndex with
                //                              | -1 
                //                              | 0 -> ""
                //                              | _ ->  code.Substring(0,separatorLastIndex)
                //
                //    ancestorAccountCode


            }

            return accountingPlanTreeNode;
        }
    }
}
