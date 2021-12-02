namespace BudgetManager.Module.Controllers
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
            this.LinkBudgetValuesController = new DevExpress.ExpressApp.Actions.PopupWindowShowAction(this.components);
            // 
            // LinkBudgetValuesController
            // 
            this.LinkBudgetValuesController.AcceptButtonCaption = null;
            this.LinkBudgetValuesController.CancelButtonCaption = null;
            this.LinkBudgetValuesController.Caption = "Link Budget Values";
            this.LinkBudgetValuesController.ConfirmationMessage = null;
            this.LinkBudgetValuesController.Id = "LinkBudgetValuesController";
            this.LinkBudgetValuesController.TargetObjectType = typeof(Budget.Model.Sql.Company);
            this.LinkBudgetValuesController.TargetViewType = DevExpress.ExpressApp.ViewType.DetailView;
            this.LinkBudgetValuesController.ToolTip = null;
            this.LinkBudgetValuesController.TypeOfView = typeof(DevExpress.ExpressApp.DetailView);
            this.LinkBudgetValuesController.CustomizePopupWindowParams += new DevExpress.ExpressApp.Actions.CustomizePopupWindowParamsEventHandler(this.LinkBudgetValuesController_CustomizePopupWindowParams);
            this.LinkBudgetValuesController.Execute += new DevExpress.ExpressApp.Actions.PopupWindowShowActionExecuteEventHandler(this.LinkBudgetValuesController_Execute);
            // 
            // LinkBudgetValues
            // 
            this.Actions.Add(this.LinkBudgetValuesController);

        }

        #endregion

        private DevExpress.ExpressApp.Actions.PopupWindowShowAction LinkBudgetValuesController;
    }
}
