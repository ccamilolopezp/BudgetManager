using System.Collections.Generic;

namespace Hierarchy.Model
{
    public interface INode
    {
        string Label { get; set; }
        bool IsHeadNode { get; set; }
        bool IsEndNode { get; set; }
        bool IsOppositeSign { get; set; }

        ITree GetTree();
    }
}
