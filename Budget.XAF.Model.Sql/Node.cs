using DevExpress.Persistent.BaseImpl.EF;
using Hierarchy.Model;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Budget.XAF.Model.Sql
{
    public class Node : HCategory, INode
    {
        [NotMapped, Browsable(false)]
        public bool LastNode => Children == null ? true : Children.Count == 0;

        public virtual Tree Tree { get; set; }

        public string Label { get; set; }
        public bool IsHeadNode { get; set; }
        public bool IsEndNode { get; set; }
        public bool IsOppositeSign { get; set; }

        public ITree GetTree() => Tree as ITree;
    }
}
