using BudgetManager.Module.BusinessManagers.Infrastructure;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.BaseImpl.EF.PermissionPolicy;
using System.Collections.Generic;

namespace BudgetManager.Module.Controllers
{
    public partial class DeleteValues : ViewController
    {
        public DeleteValues()
        {
            InitializeComponent();
        }

        protected override void OnActivated()
        {
            base.OnActivated();
            List<string> LinkBudgetValuesRoles = new List<string> { "Link Budget Values", "Administrators" };
            PermissionPolicyUser User = ObjectSpace.GetObjectByKey<PermissionPolicyUser>(SecuritySystem.CurrentUserId);
            LinkBudgetValuesController.Active["Active"] = GeneralUtils.IsRoleInUser(User, LinkBudgetValuesRoles);
        }

        private void LinkBudgetValuesController_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            IObjectSpace os = Application.CreateObjectSpace();
            BusinessObjects.NonPersistent.LinkValuesParameters treeLoadParameters = new BusinessObjects.NonPersistent.LinkValuesParameters();
            os.CommitChanges();

            e.Context = TemplateContext.PopupWindow;
            DetailView dv = Application.CreateDetailView(os, treeLoadParameters);
            dv.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.Edit;
            e.View = dv;
        }

        private void LinkBudgetValuesController_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            BusinessObjects.NonPersistent.LinkValuesParameters loadParameters = (BusinessObjects.NonPersistent.LinkValuesParameters)e.PopupWindowViewCurrentObject;
            (new BusinessManagers.ValidationManager()).ValidateLinkExecutedBudgetParameters(loadParameters);
            (new BusinessManagers.LinkManager()).LinkExecutedToBudget(loadParameters.BudgetUnit, loadParameters.ExecutedBudgetUnit);
            Application.ShowViewStrategy.ShowMessage($"Presupuesto para el año {loadParameters.BudgetUnit.Year} asociado Correctamente", InformationType.Success, 10000, InformationPosition.Bottom);
        }
    }
}
