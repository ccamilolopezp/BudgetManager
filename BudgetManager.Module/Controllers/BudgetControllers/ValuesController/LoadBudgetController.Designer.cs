namespace BudgetManager.Module.Controllers
{
    partial class LoadBudgetController
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
            this.LoadBudget = new DevExpress.ExpressApp.Actions.PopupWindowShowAction(this.components);
            // 
            // LoadBudget
            // 
            this.LoadBudget.AcceptButtonCaption = null;
            this.LoadBudget.CancelButtonCaption = null;
            this.LoadBudget.Caption = "Load Budget";
            this.LoadBudget.ConfirmationMessage = null;
            this.LoadBudget.Id = "LoadBudgetController";
            this.LoadBudget.SelectionDependencyType = DevExpress.ExpressApp.Actions.SelectionDependencyType.RequireSingleObject;
            this.LoadBudget.TargetObjectType = typeof(Budget.Model.Sql.Company);
            this.LoadBudget.TargetViewType = DevExpress.ExpressApp.ViewType.ListView;
            this.LoadBudget.ToolTip = null;
            this.LoadBudget.TypeOfView = typeof(DevExpress.ExpressApp.ListView);
            this.LoadBudget.CustomizePopupWindowParams += new DevExpress.ExpressApp.Actions.CustomizePopupWindowParamsEventHandler(this.LoadBudget_CustomizePopupWindowParams);
            this.LoadBudget.Execute += new DevExpress.ExpressApp.Actions.PopupWindowShowActionExecuteEventHandler(this.LoadBudget_Execute);
            // 
            // LoadBudgetController
            // 
            this.Actions.Add(this.LoadBudget);

        }

        #endregion

        private DevExpress.ExpressApp.Actions.PopupWindowShowAction LoadBudget;
    }
}
