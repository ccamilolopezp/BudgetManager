namespace BudgetManager.Module.Controllers
{
    partial class LoadExecutedBudgetController
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
            this.LoadExecutedController = new DevExpress.ExpressApp.Actions.PopupWindowShowAction(this.components);
            // 
            // LoadExecutedController
            // 
            this.LoadExecutedController.AcceptButtonCaption = null;
            this.LoadExecutedController.CancelButtonCaption = null;
            this.LoadExecutedController.Caption = "Load Executed Budget";
            this.LoadExecutedController.ConfirmationMessage = null;
            this.LoadExecutedController.Id = "LoadExecutedBudgetController";
            this.LoadExecutedController.SelectionDependencyType = DevExpress.ExpressApp.Actions.SelectionDependencyType.RequireSingleObject;
            this.LoadExecutedController.TargetObjectType = typeof(Budget.Model.Sql.Company);
            this.LoadExecutedController.TargetViewType = DevExpress.ExpressApp.ViewType.ListView;
            this.LoadExecutedController.ToolTip = null;
            this.LoadExecutedController.TypeOfView = typeof(DevExpress.ExpressApp.ListView);
            this.LoadExecutedController.CustomizePopupWindowParams += new DevExpress.ExpressApp.Actions.CustomizePopupWindowParamsEventHandler(this.LoadExecutedBudgetController_CustomizePopupWindowParams);
            this.LoadExecutedController.Execute += new DevExpress.ExpressApp.Actions.PopupWindowShowActionExecuteEventHandler(this.LoadExecutedBudgetController_Execute);
            // 
            // LoadExecutedBudgetController
            // 
            this.Actions.Add(this.LoadExecutedController);

        }

        #endregion

        private DevExpress.ExpressApp.Actions.PopupWindowShowAction LoadExecutedController;
    }
}
