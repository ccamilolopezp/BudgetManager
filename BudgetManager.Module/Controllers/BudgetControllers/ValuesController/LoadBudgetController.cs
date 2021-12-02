using BudgetManager.Module.BusinessManagers.Infrastructure;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.BaseImpl.EF.PermissionPolicy;
using System.Collections.Generic;

namespace BudgetManager.Module.Controllers
{
    public partial class LoadBudgetController : ViewController
    {
        public LoadBudgetController()
        {
            InitializeComponent();
        }

        protected override void OnActivated()
        {
            base.OnActivated();
            List<string> LoadBudgetRoles = new List<string> { "Load Budget", "Administrators" };
            PermissionPolicyUser User = ObjectSpace.GetObjectByKey<PermissionPolicyUser>(SecuritySystem.CurrentUserId);
            LoadBudget.Active["Active"] = GeneralUtils.IsRoleInUser(User, LoadBudgetRoles);
        }

        private void LoadBudget_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            IObjectSpace os = Application.CreateObjectSpace();
            BusinessObjects.NonPersistent.LoadBudgetValuesParameters treeLoadParameters = new BusinessObjects.NonPersistent.LoadBudgetValuesParameters();
            os.CommitChanges();

            e.Context = TemplateContext.PopupWindow;
            DetailView dv = Application.CreateDetailView(os, treeLoadParameters);
            dv.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.Edit;
            e.View = dv;
        }

        private void LoadBudget_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            var company = (Budget.Model.Sql.Company)View.CurrentObject;
            BusinessObjects.NonPersistent.LoadBudgetValuesParameters loadParameters = (BusinessObjects.NonPersistent.LoadBudgetValuesParameters)e.PopupWindowViewCurrentObject;
            (new BusinessManagers.ValidationManager()).ValidateLoadQuotedParameters(company, loadParameters);
            (new BusinessManagers.QuotedManager()).LoadQuoted(company, loadParameters);
            Application.ShowViewStrategy.ShowMessage($"Valores para el árbol {loadParameters.LoadParameters.TreeName.Name} cargados exitosamente", InformationType.Success, 10000, InformationPosition.Bottom);
        }

    }
}
