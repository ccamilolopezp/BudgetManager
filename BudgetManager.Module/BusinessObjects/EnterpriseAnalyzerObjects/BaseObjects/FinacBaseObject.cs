using DevExpress.ExpressApp;
using DevExpress.Persistent.BaseImpl.EF.PermissionPolicy;
using System.ComponentModel.DataAnnotations.Schema;

namespace BudgetManager.Module.BusinessObjects.EnterpriseAnalyzerObjects
{
    public class FinacBaseObject : IXafEntityObject, IObjectSpaceLink
    {
        //[Browsable(false)]
        //public Int32 ID { get; protected set; }
        [NotMapped]
        public bool IsNew { get; protected set; }
        public virtual PermissionPolicyUser CreatedBy { get; protected set; }
        public virtual PermissionPolicyUser LastModifiedBy { get; protected set; }
        #region IXafEntityObject members
        void IXafEntityObject.OnCreated()
        {
            CreatedBy = objectSpace.GetObjectByKey<PermissionPolicyUser>(SecuritySystem.CurrentUserId);
            IsNew = true;
        }
        void IXafEntityObject.OnLoaded()
        {
            IsNew = false;
        }
        void IXafEntityObject.OnSaving()
        {
            if (objectSpace != null)
            {
                LastModifiedBy = objectSpace.GetObjectByKey<PermissionPolicyUser>(SecuritySystem.CurrentUserId);
            }
            IsNew = false;
        }
        #endregion
        #region IObjectSpaceLink members
        private IObjectSpace objectSpace;
        IObjectSpace IObjectSpaceLink.ObjectSpace
        {
            get { return objectSpace; }
            set { objectSpace = value; }
        }
        #endregion
    }
}
