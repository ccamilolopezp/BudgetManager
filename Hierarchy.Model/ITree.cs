using System.Collections.Generic;

namespace Hierarchy.Model
{
    public interface ITree
    {
        string Name { get; set; }
        string Code { get; set; }
        ITreeName GetTreeName();
        IList<INode> GetNodes();
    }
}
