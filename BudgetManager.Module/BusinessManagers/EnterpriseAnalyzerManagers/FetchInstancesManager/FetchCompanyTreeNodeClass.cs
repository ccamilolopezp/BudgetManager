using BudgetManager.Module.BusinessObjects.EnterpriseAnalyzerObjects;
using DevExpress.ExpressApp;
using System;
using System.Linq;

namespace BudgetManager.Module.BusinessManagers.EnterpriseAnalyzerManagers
{
    class FetchCompanyTreeNodeClass

    {
        public CompanyTreeNode FetchCompanyTreeNode(IObjectSpace ios,
                                                    string companyTreeNodeName,
                                                    string companyName,
                                                    string callerMethodName)
        {
            Company company = (new FetchCompanyClass()).FetchCompany(ios,
                                                                     companyName,
                                                                     false,
                                                                     callerMethodName);

            CompanyTreeNode companyTreeNode = FetchCompanyTreeNode(ios,
                                                                   companyTreeNodeName,
                                                                   company,
                                                                   callerMethodName);

            return companyTreeNode;
        }

        public CompanyTreeNode FetchCompanyTreeNode(IObjectSpace ios,
                                                    string companyTreeNodeLabel,
                                                    Company company,
                                                    string callerMethodName)
        {
            CompanyTreeNode companyTreeNode = ios.GetObjects<CompanyTreeNode>()
                                              .Where(ctn => ctn.CompanyTree.Company.Name == company.Name && ctn.Label == companyTreeNodeLabel)
                                              .FirstOrDefault();

            if (companyTreeNode == null)
            {
                throw new Exception("("
                                    + callerMethodName
                                    + ") No existe CompanyTreeNode: "
                                    + companyTreeNodeLabel
                                    + " , "
                                    + company.Name);
            }

            return companyTreeNode;
        }

    }
}
