using Finac.Business.Model;
using Hierarchy.Model;
using System.Collections.Generic;
using System.Linq;

namespace Budget.XAF.Model.Sql
{
    public class Tree : IEntity<long>, ITree
    {
        public virtual TreeName TreeName { get; set; }
        public virtual List<Node> Nodes { get; set; }

        public long Id { get; set; }

        public string Name { get; set; }
        public string Code { get; set; }
        public IList<INode> GetNodes() => Nodes.OfType<INode>().ToList();
        public ITreeName GetTreeName() => TreeName as ITreeName;
    }
}
