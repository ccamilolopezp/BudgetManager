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

    [NavigationItem("Trees -")]
    [DefaultClassOptions]
    public class ValueModel : CompanyDataObject
    {
        public ValueModel()
        {
            ValueModelFrames = new List<ValueModelFrame>();
            Companies = new List<Company>();
        }

        [Browsable(false)]
        public Int32 ValueModelId { get; protected set; }

        [Aggregated]
        public virtual IList<ValueModelFrame> ValueModelFrames { get; set; }

        public virtual IList<Company> Companies { get; set; }

        public string Name { get; set; }
    }

    [NavigationItem("Trees -")]
    [DefaultClassOptions]
    public class ValueModelFrame : FinacBaseObject
    {
        [Browsable(false)]
        public Int32 ValueModelFrameId { get; protected set; }
        public virtual ValueModel ValueModel { get; set; }

        public virtual ValueModelTree ValueModelTree { get; set; }
        public string Name { get; set; }


    }

    [NavigationItem("Trees -")]
    [DefaultClassOptions]
    public class ValueModelTree : FinacBaseObject
    {

        [Browsable(false)]
        public Int32 ValueModelTreeId { get; protected set; }
        public ValueModelTree()
        {
            ValueModelTreeNodes = new List<ValueModelTreeNode>();
        }


        [Aggregated]
        public virtual IList<ValueModelTreeNode> ValueModelTreeNodes { get; set; }

        public string Name { get; set; }

        public string AccountPrefix { get; set; }

        public int PeriodInMonths { get; set; }
    }

    [NavigationItem("Trees -")]
    [DefaultClassOptions]
    [Appearance("FontColorBlue", AppearanceItemType = "ViewItem", TargetItems = "*", Context = "ListView",
    Criteria = "IsHeadNode", FontColor = "Blue")]
    [Appearance("BackColorGreen", AppearanceItemType = "ViewItem", TargetItems = "*", Context = "ListView",
    Criteria = "IsEndNode", FontColor = "Green")]
    public class ValueModelTreeNode : HCategory, IXafEntityObject, IObjectSpaceLink
    {
        public virtual ValueModelTree ValueModelTree { get; set; }

        public ValueModelTreeNode()
        {
            ValueModelTreeNodeAncestors = new List<ValueModelTreeNodeAncestor>();
            ValueModelTreeNodeFormulaSteps = new List<ValueModelTreeNodeFormulaStep>();
        }
        [Aggregated]
        public virtual IList<ValueModelTreeNodeAncestor> ValueModelTreeNodeAncestors { get; set; }

        [Aggregated]
        public virtual IList<ValueModelTreeNodeFormulaStep> ValueModelTreeNodeFormulaSteps { get; set; }

        public string Label { get; set; }
        public bool IsHeadNode { get; set; }
        public bool IsEndNode { get; set; }
        public bool IsOppositeSign { get; set; }

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
                    objectSpace.Delete(this.ValueModelTreeNodeAncestors);

                    ValueModelTreeNode currentNode = this;

                    bool currentNodeHasParent = true;
                    int seniority = 0;

                    ValueModelTreeNodeAncestor treeNodeAncestor = objectSpace.CreateObject<ValueModelTreeNodeAncestor>(); // new TreeNodeAncestor(Session);
                    treeNodeAncestor.ValueModelTreeNode = this;
                    treeNodeAncestor.AncestorValueModelTreeNode = this;
                    treeNodeAncestor.Name = this.Name;
                    treeNodeAncestor.Seniority = seniority;
                    //treeNodeAncestor.IsExternal = this.IsExternal;
                    ValueModelTreeNodeAncestors.Add(treeNodeAncestor);

                    do
                    {
                        seniority += 1;

                        if (!currentNode.IsHeadNode)
                        {
                            treeNodeAncestor = objectSpace.CreateObject<ValueModelTreeNodeAncestor>(); // new TreeNodeAncestor(Session);
                            treeNodeAncestor.ValueModelTreeNode = this;

                            if (currentNode.Parent != null)
                            {
                                treeNodeAncestor.Name = currentNode.Parent.Name;
                                treeNodeAncestor.Seniority = seniority;
                                //treeNodeAncestor.IsExternal = ((StructuralTreeNode)currentNode.Parent).IsExternal;
                                treeNodeAncestor.AncestorValueModelTreeNode = (ValueModelTreeNode)currentNode.Parent;

                                ValueModelTreeNodeAncestors.Add(treeNodeAncestor);
                                currentNode = (ValueModelTreeNode)currentNode.Parent;
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

    public class ValueModelTreeNodeAncestor : FinacBaseObject
    {
        public virtual ValueModelTreeNode ValueModelTreeNode { get; set; }

        [Browsable(false)]
        public Int32 ID { get; protected set; }

        public string Name { get; set; }

        public int Seniority { get; set; }

        public bool IsExternal { get; set; }

        public ValueModelTreeNode AncestorValueModelTreeNode { get; set; }
    }

    public class ValueModelTreeNodeFormulaStep : FinacBaseObject
    {
        public virtual ValueModelTreeNode ValueModelTreeNode { get; set; }

        [Browsable(false)]
        public Int32 ID { get; protected set; }

        public int Order { get; set; }

        public string Step { get; set; }
    }
}
