using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.ExpressApp.Data;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl.EF;
using DevExpress.Persistent.BaseImpl.EF.PermissionPolicy;

namespace BudgetManager.Module.BusinessObjects.EnterpriseAnalyzerObjects
{
    [NavigationItem("Z-Companies -")]
    [DefaultClassOptions]
    [System.ComponentModel.DefaultProperty("Company.Name")]

    public class CompanyTree : FinacBaseObject
    {
        public CompanyTree()
        {
            CompanyTreeNodes = new List<CompanyTreeNode>();
        }

        [Browsable(false)]
        [Key, ForeignKey("Company")]
        public Int32 CompanyTreeId { get; protected set; }

        [Aggregated]
        public virtual IList<CompanyTreeNode> CompanyTreeNodes { get; set; }

        public string Name { get; set; }
        public virtual Company Company { get; set; }
    }

    [NavigationItem("Z-Companies -")]
    [DefaultClassOptions]
    [Appearance("BackColorPink", AppearanceItemType = "ViewItem", TargetItems = "*", Context = "ListView",
    Criteria = "IsExternal", BackColor = "Pink")]
    [Appearance("FontColorBlue", AppearanceItemType = "ViewItem", TargetItems = "*", Context = "ListView",
    Criteria = "IsHeadNode", FontColor = "Blue")]
    [Appearance("BackColorGreen", AppearanceItemType = "ViewItem", TargetItems = "*", Context = "ListView",
    Criteria = "IsEndNode", FontColor = "Green")]
    [Appearance("FontColorRed", AppearanceItemType = "ViewItem", TargetItems = "*", Context = "ListView",
    Criteria = "IsActivityCenterNode", FontColor = "Red")]
    public class CompanyTreeNode : HCategory, IXafEntityObject, IObjectSpaceLink
    {
        public virtual CompanyTree CompanyTree { get; set; }

        public CompanyTreeNode()
        {
            CompanyTreeNodeAncestors = new List<CompanyTreeNodeAncestor>();
        }
        [Aggregated]
        public virtual IList<CompanyTreeNodeAncestor> CompanyTreeNodeAncestors { get; set; }

        public string Label { get; set; }
        public bool IsHeadNode { get; set; }
        public bool IsEndNode { get; set; }
        public bool IsActivityCenterNode { get; set; }
        public bool CreateAncestorsOnSaving { get; set; }

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

                //base.OnSaving();

                objectSpace.Delete(this.CompanyTreeNodeAncestors);

                CompanyTreeNode currentNode = this;

                bool currentNodeHasParent = true;
                int seniority = 0;

                CompanyTreeNodeAncestor treeNodeAncestor = objectSpace.CreateObject<CompanyTreeNodeAncestor>(); // new TreeNodeAncestor(Session);
                treeNodeAncestor.CompanyTreeNode = this;
                treeNodeAncestor.AncestorCompanyTreeNode = this;
                treeNodeAncestor.Name = this.Name;
                treeNodeAncestor.Seniority = seniority;
                this.CompanyTreeNodeAncestors.Add(treeNodeAncestor);

                do
                {
                    seniority += 1;

                    if (!currentNode.IsHeadNode)
                    {
                        treeNodeAncestor = objectSpace.CreateObject<CompanyTreeNodeAncestor>(); // new TreeNodeAncestor(Session);
                        treeNodeAncestor.CompanyTreeNode = this;

                        if (currentNode.Parent != null)
                        {
                            treeNodeAncestor.Name = currentNode.Parent.Name;
                            treeNodeAncestor.Seniority = seniority;
                            treeNodeAncestor.AncestorCompanyTreeNode = (CompanyTreeNode)currentNode.Parent;

                            this.CompanyTreeNodeAncestors.Add(treeNodeAncestor);
                            currentNode = (CompanyTreeNode)currentNode.Parent;
                        }
                        else
                        {
                            currentNodeHasParent = false;
                        }

                        /////////////////////////////
                        //treeNodeAncestor = new TreeNodeAncestor(Session);
                        //treeNodeAncestor.CompanyTreeNode = this;

                        //if (currentNode.Parent != null)
                        //{
                        //    treeNodeAncestor.Name = currentNode.Parent.Name;
                        //    treeNodeAncestor.Seniority = seniority;
                        //    treeNodeAncestor.IsExternal = ((CompanyTreeNode)currentNode.Parent).IsExternal;
                        //    treeNodeAncestor.AncestorCompanyTreeNode = (CompanyTreeNode)currentNode.Parent;

                        //    TreeNodeAncestors.Add(treeNodeAncestor);
                        //    currentNode = (CompanyTreeNode)currentNode.Parent;
                        //}
                        //else
                        //{
                        //    currentNodeHasParent = false;
                        //}

                        ///////////////////////////////

                    }

                } while (!currentNode.IsHeadNode && currentNodeHasParent);
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

    [NavigationItem("Z-Companies -")]
    [DefaultClassOptions]
    public class CompanyTreeNodeAncestor : FinacBaseObject
    {
        [Browsable(false)]
        public Int32 CompanyTreeNodeAncestorId { get; protected set; }
        public CompanyTreeNodeAncestor() { }
        public virtual CompanyTreeNode CompanyTreeNode { get; set; }

        public string Name { get; set; }

        public int Seniority { get; set; }

        public bool IsExternal { get; set; }

        public virtual CompanyTreeNode AncestorCompanyTreeNode { get; set; }
    }
}
