using DevExpress.Persistent.Base;

namespace BudgetManager.Module.BusinessObjects.EnterpriseAnalyzerObjects
{
    [NavigationItem("Z-Companies -")]
    [DefaultClassOptions]
    public class CompanyDataObject : FinacBaseObject
    {

        //[Appearance("CDO1", Visibility = ViewItemVisibility.Hide)]

        public CompanyTreeNode CompanyTreeNode { get; set; }

        //[Appearance("CDO2", Visibility = ViewItemVisibility.Hide)]
        public string CompanyName { get; set; }

        //[Appearance("CDO3", Visibility = ViewItemVisibility.Hide)]
        public bool IsCompanyLevelObject { get; set; }


    }
}
