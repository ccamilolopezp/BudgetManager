using BudgetManager.Module.BusinessObjects.EnterpriseAnalyzerObjects;
using DevExpress.ExpressApp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Text;

namespace BudgetManager.Module.BusinessManagers.EnterpriseAnalyzerManagers
{
    class LoadAlcaparrosCompanyTreeClass
    {
        public void LoadAlcaparrosCompanyTree(IObjectSpace ios)
        {
            //IList<CompanyTree> companyTreeToDelete = ios.GetObjects<CompanyTree>();
            //ios.Delete(companyTreeToDelete);

            string fileRelaciónSolvencia = ConfigurationManager.AppSettings["CompanyTree"];
            var recordsRelaciónSolvencia = File.ReadLines(fileRelaciónSolvencia, Encoding.UTF8)
                .Select(d => RecordConversion.CreateLiteCompanyTreeRecord(d)).ToObservable();

            var company = ios.GetObjects<Company>().Where(o => o.Name == "Alcaparros").FirstOrDefault();

            List<string[]> linputRecords = new List<string[]>();
            Dictionary<string, CompanyTreeNode> companyTreeNodeDictionary = new Dictionary<string, CompanyTreeNode>();

            if (company.CompanyTree != null)
            {
                var observableCompanyTreeNodes = company.CompanyTree.CompanyTreeNodes.ToObservable();
                observableCompanyTreeNodes.Subscribe(d =>
                {
                    ios.Delete(d.CompanyTreeNodeAncestors);
                });
                ios.Delete(company.CompanyTree.CompanyTreeNodes);
                ios.Delete(company.CompanyTree);
            }

            CompanyTree companyTree = ios.CreateObject<CompanyTree>();
            //companyTree.CompanyName = company.Name;
            companyTree.Company = company;

            company.CompanyTree = companyTree;

            recordsRelaciónSolvencia.Subscribe(d =>
            {
                if (d.Item1 == "Util")
                {
                    if (d.Item2[0] == company.Name)
                    {
                        linputRecords.Add(d.Item2);

                        CompanyTreeNode Hobj = ios.CreateObject<CompanyTreeNode>();
                        Hobj.CompanyTree = companyTree;
                        Hobj.Label = d.Item2[2];
                        Hobj.Name = d.Item2[1];
                        Hobj.IsHeadNode = d.Item2[5] == "S"; //element.Parameter6 == "S";
                        Hobj.IsEndNode = d.Item2[6] == "S"; //element.Parameter7 == "S";

                        companyTreeNodeDictionary.Add(Hobj.Name, Hobj);
                    }
                }
            });

            var observableInputRecords = linputRecords.Where(o => o[4] != "0").ToObservable();

            observableInputRecords.Subscribe(d =>
            {
                string companyTreeNodeCode = d[1];
                CompanyTreeNode companyTreeNode = null;
                if (companyTreeNodeDictionary.TryGetValue(companyTreeNodeCode, out companyTreeNode)) // Returns true.
                {
                    string ancestorCode = d[4];
                    CompanyTreeNode ancestorTreeNode = null;
                    if (companyTreeNodeDictionary.TryGetValue(ancestorCode, out ancestorTreeNode)) // Returns true.
                    {
                        companyTreeNode.Parent = ancestorTreeNode;
                    }
                    else
                    {
                        throw new Exception("(LoadCompanyTreeClass) No hay tree node ancestro con código: " + ancestorCode);
                    }
                }
                else
                {
                    throw new Exception("(LoadCompanyTreeClass) No hay company tree node con código: " + companyTreeNodeCode);
                }
            });

            var observableEndNodes = companyTreeNodeDictionary
                                     .Select(o => o.Value)
                                     .Where(o => o.IsEndNode)
                                     .ToObservable();

            observableEndNodes.Subscribe(d =>
            {
                (new CreateActivityCenterClass()).CreateDefaultNewActivityCenter(ios, d, company.CompanyTree);

            });

        }

    }
}
