using BudgetManager.Module.BusinessManagers.Infrastructure;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.BaseImpl.EF.PermissionPolicy;
using System.Collections.Generic;

namespace BudgetManager.Module.Controllers
{
    public partial class LoadBudgetTreesController : ViewController
    {
        public LoadBudgetTreesController()
        {
            InitializeComponent();
        }

        protected override void OnActivated()
        {
            base.OnActivated();
            List<string> LoadBudgetTreeRoles = new List<string> { "Load Budget Tree", "Administrators" };
            PermissionPolicyUser User = ObjectSpace.GetObjectByKey<PermissionPolicyUser>(SecuritySystem.CurrentUserId);
            LoadBudgetTreeController.Active["Active"] = GeneralUtils.IsRoleInUser(User, LoadBudgetTreeRoles);
        }

        private void LoadBudgetTreeController_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            IObjectSpace os = Application.CreateObjectSpace();
            BusinessObjects.NonPersistent.LoadBudgetTreeParameters treeLoadParameters = new BusinessObjects.NonPersistent.LoadBudgetTreeParameters();
            os.CommitChanges();

            e.Context = TemplateContext.PopupWindow;
            DetailView dv = Application.CreateDetailView(os, treeLoadParameters);
            dv.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.Edit;
            e.View = dv;
        }

        private void LoadBudgetTreeController_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            var company = (Budget.Model.Sql.Company)View.CurrentObject;
            BusinessObjects.NonPersistent.LoadBudgetTreeParameters parameters = (BusinessObjects.NonPersistent.LoadBudgetTreeParameters)e.PopupWindowViewCurrentObject;
            (new BusinessManagers.ValidationManager()).ValidateLoadBudgetTreeParameters(company, parameters);
            (new BusinessManagers.TreeManagers.BudgetTreeManager()).LoadBudgetTree(company, parameters);
            Application.ShowViewStrategy.ShowMessage($"Estructuradel arbol {parameters.LoadParameters.TreeName.Name} cargada exitosamente", InformationType.Success, 10000, InformationPosition.Bottom);
        }
    }
}
