using BudgetManager.Module.BusinessManagers.Infrastructure;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.BaseImpl.EF.PermissionPolicy;
using System.Collections.Generic;

namespace BudgetManager.Module.Controllers
{
    public partial class LoadExecutedBudgetController : ViewController
    {
        public LoadExecutedBudgetController()
        {
            InitializeComponent();
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            List<string> LoadExecutedRoles = new List<string> { "Load Executed Budget", "Administrators" };
            PermissionPolicyUser User = ObjectSpace.GetObjectByKey<PermissionPolicyUser>(SecuritySystem.CurrentUserId);
            LoadExecutedController.Active["Active"] = GeneralUtils.IsRoleInUser(User, LoadExecutedRoles);
        }

        private void LoadExecutedBudgetController_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            IObjectSpace os = Application.CreateObjectSpace();
            BusinessObjects.NonPersistent.LoadExecutedBudgetValuesParameters treeLoadParameters = new BusinessObjects.NonPersistent.LoadExecutedBudgetValuesParameters();
            os.CommitChanges();

            e.Context = TemplateContext.PopupWindow;
            DetailView dv = Application.CreateDetailView(os, treeLoadParameters);
            dv.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.Edit;
            e.View = dv;
        }

        private void LoadExecutedBudgetController_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            var company = (Budget.Model.Sql.Company)View.CurrentObject;
            BusinessObjects.NonPersistent.LoadExecutedBudgetValuesParameters loadParameters = (BusinessObjects.NonPersistent.LoadExecutedBudgetValuesParameters)e.PopupWindowViewCurrentObject;
            (new BusinessManagers.ValidationManager()).ValidateLoadExecutedParameters(company, loadParameters);
            (new BusinessManagers.ExecutedBudgetManager()).LoadPeriodExecuted(company, loadParameters, loadParameters.ProcessDate);
            Application.ShowViewStrategy.ShowMessage($"Valores del presupuesto Ejecutado para la fecha {loadParameters.ProcessDate.ToShortDateString()} cargados exitosamente", InformationType.Success, 10000, InformationPosition.Bottom);
        }
    }
}
