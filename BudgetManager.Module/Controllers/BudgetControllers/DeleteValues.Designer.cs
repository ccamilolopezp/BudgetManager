namespace BudgetManager.Module.Controllers.BudgetControllers
{
    partial class DeleteValues
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.DeleteValuesController = new DevExpress.ExpressApp.Actions.PopupWindowShowAction(this.components);
            // 
            // DeleteValuesController
            // 
            this.DeleteValuesController.AcceptButtonCaption = null;
            this.DeleteValuesController.ActionMeaning = DevExpress.ExpressApp.Actions.ActionMeaning.Accept;
            this.DeleteValuesController.CancelButtonCaption = null;
            this.DeleteValuesController.Caption = "Delete Values";
            this.DeleteValuesController.Category = "Edit";
            this.DeleteValuesController.ConfirmationMessage = null;
            this.DeleteValuesController.Id = "DeleteValuesController";
            this.DeleteValuesController.SelectionDependencyType = DevExpress.ExpressApp.Actions.SelectionDependencyType.RequireSingleObject;
            this.DeleteValuesController.TargetObjectType = typeof(Budget.Model.Sql.Company);
            this.DeleteValuesController.TargetViewType = DevExpress.ExpressApp.ViewType.DetailView;
            this.DeleteValuesController.ToolTip = null;
            this.DeleteValuesController.TypeOfView = typeof(DevExpress.ExpressApp.DetailView);
            this.DeleteValuesController.CustomizePopupWindowParams += new DevExpress.ExpressApp.Actions.CustomizePopupWindowParamsEventHandler(this.DeleteValuesController_CustomizePopupWindowParams);
            this.DeleteValuesController.Execute += new DevExpress.ExpressApp.Actions.PopupWindowShowActionExecuteEventHandler(this.DeleteValuesController_Execute);
            // 
            // DeleteValues
            // 
            this.Actions.Add(this.DeleteValuesController);

        }

        #endregion

        private DevExpress.ExpressApp.Actions.PopupWindowShowAction DeleteValuesController;
    }
}
