using BudgetManager.Module.BusinessObjects.EnterpriseAnalyzerObjects;
using DevExpress.ExpressApp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BudgetManager.Module.BusinessManagers.EnterpriseAnalyzerManagers
{
    class FetchCompanyClass
    {
        public Company FetchCompany(IObjectSpace ios, string name, bool isOptionFetch, string callerMethod)
        {
            Company company = ios.GetObjects<Company>().Where(o => o.Name == name).FirstOrDefault();

            if (!isOptionFetch & company == null)
            {
                throw new Exception("(" + callerMethod + ") No encontró Company con Name: " + name);
            }

            return company;
        }

        public AugmentedCompany FetchAugmentedCompany(IObjectSpace ios, string name, bool isOptionFetch, string callerMethod)
        {
            AugmentedCompany company = ios.GetObjects<AugmentedCompany>().Where(o => o.Name == name).FirstOrDefault();

            if (!isOptionFetch & company == null)
            {
                throw new Exception("(" + callerMethod + ") No encontró AugmentedCompany con Name: " + name);
            }

            return company;
        }

        public List<Company> FetchCompanyList(IObjectSpace ios, string callerMethod)
        {
            List<Company> lCompany = ios.GetObjects<Company>().ToList();

            return lCompany;
        }


    }
}
