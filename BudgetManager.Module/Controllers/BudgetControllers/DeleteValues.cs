using BudgetManager.Module.BusinessManagers.Infrastructure;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.BaseImpl.EF.PermissionPolicy;
using System.Collections.Generic;

namespace BudgetManager.Module.Controllers.BudgetControllers
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
            List<string> DeleteValuesRoles = new List<string> { "Delete Values", "Administrators" };
            PermissionPolicyUser User = ObjectSpace.GetObjectByKey<PermissionPolicyUser>(SecuritySystem.CurrentUserId);
            DeleteValuesController.Active["Active"] = GeneralUtils.IsRoleInUser(User, DeleteValuesRoles);
        }

        private void DeleteValuesController_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            IObjectSpace os = Application.CreateObjectSpace();
            BusinessObjects.NonPersistent.DeleteParameters deleteParameters = new BusinessObjects.NonPersistent.DeleteParameters();
            os.CommitChanges();

            e.Context = TemplateContext.PopupWindow;
            DetailView dv = Application.CreateDetailView(os, deleteParameters);
            dv.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.Edit;
            e.View = dv;
        }

        private void DeleteValuesController_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {             
            BusinessObjects.NonPersistent.DeleteParameters deleteParameters = (BusinessObjects.NonPersistent.DeleteParameters)e.PopupWindowViewCurrentObject;
            (new BusinessManagers.ValidationManager()).ValidateDeleteParameters(deleteParameters);
            (new BusinessManagers.Infrastructure.DeleteManager()).DeleteValues(deleteParameters);
            Application.ShowViewStrategy.ShowMessage($"Valores Eliminados Correctamente", InformationType.Success, 10000, InformationPosition.Bottom);
        }
    }
}
