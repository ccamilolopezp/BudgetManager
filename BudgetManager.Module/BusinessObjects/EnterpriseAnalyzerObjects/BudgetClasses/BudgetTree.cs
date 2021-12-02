using DevExpress.ExpressApp;
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl.EF;
using DevExpress.Persistent.BaseImpl.EF.PermissionPolicy;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace BudgetManager.Module.BusinessObjects.EnterpriseAnalyzerObjects
{
    [NavigationItem("Z-Companies -")]
    [DefaultClassOptions]
    [System.ComponentModel.DefaultProperty("Company.Name")]
    public class BudgetTree : CompanyDataObject
    {
        [Browsable(false)]
        [ForeignKey("Company")]
        public Int32 BudgetTreeId { get; protected set; }
        public virtual AugmentedCompany Company { get; set; }

        public BudgetTree()
        {
            BudgetTreeNodes = new List<BudgetTreeNode>();
        }

        [Aggregated]
        public virtual IList<BudgetTreeNode> BudgetTreeNodes { get; set; }
    }

    [NavigationItem("Z-Companies -")]
    [DefaultClassOptions]
    [Appearance("BackColorPink", AppearanceItemType = "ViewItem", TargetItems = "*", Context = "ListView",
    Criteria = "IsExternal", BackColor = "Pink")]
    [Appearance("FontColorBlue", AppearanceItemType = "ViewItem", TargetItems = "*", Context = "ListView",
    Criteria = "IsHeadNode", FontColor = "Blue")]
    [Appearance("BackColorGreen", AppearanceItemType = "ViewItem", TargetItems = "*", Context = "ListView",
    Criteria = "IsEndNode", FontColor = "Green")]
    [System.ComponentModel.DefaultProperty("Name")]
    public class BudgetTreeNode : HCategory, IXafEntityObject, IObjectSpaceLink
    {
        [Browsable(false)]
        public Int32 BudgetTreeNodeId { get; protected set; }
        public virtual BudgetTree BudgetTree { get; set; }

        public BudgetTreeNode()
        {
            BudgetTreeNodeAncestors = new List<BudgetTreeNodeAncestor>();
            //BudgetTreeNodeFormulaSteps = new List<BudgetTreeNodeFormulaStep>();
        }
        [Aggregated]
        public virtual IList<BudgetTreeNodeAncestor> BudgetTreeNodeAncestors { get; set; }


        public string Label { get; set; }
        public bool IsHeadNode { get; set; }
        public bool IsEndNode { get; set; }
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

                if (this.CreateAncestorsOnSaving)
                {
                    objectSpace.Delete(this.BudgetTreeNodeAncestors);

                    BudgetTreeNode currentNode = this;

                    bool currentNodeHasParent = true;
                    int seniority = 0;

                    BudgetTreeNodeAncestor treeNodeAncestor = objectSpace.CreateObject<BudgetTreeNodeAncestor>(); // new TreeNodeAncestor(Session);
                    treeNodeAncestor.BudgetTreeNode = this;
                    treeNodeAncestor.AncestorBudgetTreeNode = this;
                    treeNodeAncestor.Name = this.Name;
                    treeNodeAncestor.Seniority = seniority;
                    //treeNodeAncestor.IsExternal = this.IsExternal;
                    BudgetTreeNodeAncestors.Add(treeNodeAncestor);

                    do
                    {
                        seniority += 1;

                        if (!currentNode.IsHeadNode)
                        {
                            treeNodeAncestor = objectSpace.CreateObject<BudgetTreeNodeAncestor>(); // new TreeNodeAncestor(Session);
                            treeNodeAncestor.BudgetTreeNode = this;

                            if (currentNode.Parent != null)
                            {
                                treeNodeAncestor.Name = currentNode.Parent.Name;
                                treeNodeAncestor.Seniority = seniority;
                                //treeNodeAncestor.IsExternal = ((StructuralTreeNode)currentNode.Parent).IsExternal;
                                treeNodeAncestor.AncestorBudgetTreeNode = (BudgetTreeNode)currentNode.Parent;

                                BudgetTreeNodeAncestors.Add(treeNodeAncestor);
                                currentNode = (BudgetTreeNode)currentNode.Parent;
                            }
                            else
                            {
                                currentNodeHasParent = false;
                            }

                        }

                    } while (!currentNode.IsHeadNode && currentNodeHasParent);
                }
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

    public class BudgetTreeNodeAncestor : FinacBaseObject
    {
        public virtual BudgetTreeNode BudgetTreeNode { get; set; }

        [Browsable(false)]
        public Int32 ID { get; protected set; }

        public string Name { get; set; }

        public int Seniority { get; set; }

        public bool IsExternal { get; set; }

        public virtual BudgetTreeNode AncestorBudgetTreeNode { get; set; }
    }
}
