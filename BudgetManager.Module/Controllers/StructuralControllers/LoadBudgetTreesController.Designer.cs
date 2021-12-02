namespace BudgetManager.Module.Controllers
{
    partial class LoadBudgetTreesController
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
            this.LoadBudgetTreeController = new DevExpress.ExpressApp.Actions.PopupWindowShowAction(this.components);
            // 
            // LoadBudgetTreeController
            // 
            this.LoadBudgetTreeController.AcceptButtonCaption = null;
            this.LoadBudgetTreeController.CancelButtonCaption = null;
            this.LoadBudgetTreeController.Caption = "Load Budget Tree";
            this.LoadBudgetTreeController.ConfirmationMessage = null;
            this.LoadBudgetTreeController.Id = "LoadBudgetTreeController";
            this.LoadBudgetTreeController.SelectionDependencyType = DevExpress.ExpressApp.Actions.SelectionDependencyType.RequireSingleObject;
            this.LoadBudgetTreeController.TargetObjectType = typeof(Budget.Model.Sql.Company);
            this.LoadBudgetTreeController.TargetViewType = DevExpress.ExpressApp.ViewType.ListView;
            this.LoadBudgetTreeController.ToolTip = null;
            this.LoadBudgetTreeController.TypeOfView = typeof(DevExpress.ExpressApp.ListView);
            this.LoadBudgetTreeController.CustomizePopupWindowParams += new DevExpress.ExpressApp.Actions.CustomizePopupWindowParamsEventHandler(this.LoadBudgetTreeController_CustomizePopupWindowParams);
            this.LoadBudgetTreeController.Execute += new DevExpress.ExpressApp.Actions.PopupWindowShowActionExecuteEventHandler(this.LoadBudgetTreeController_Execute);
            // 
            // LoadBudgetTree
            // 
            this.Actions.Add(this.LoadBudgetTreeController);

        }

        #endregion

        private DevExpress.ExpressApp.Actions.PopupWindowShowAction LoadBudgetTreeController;
    }
}
