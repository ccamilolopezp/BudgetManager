using Finac.Business.Model;
using Hierarchy.Model;

namespace Budget.XAF.Model.Sql
{
    public class TreeName : IEntity<long>, ITreeName
    {
        public virtual Tree Tree { get; set; }

        public long Id { get; set; }

        public string Name { get; set; }
        public ITree GetTree() => Tree as ITree;
    }
}
