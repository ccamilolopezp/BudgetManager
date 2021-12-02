namespace BudgetManager.Module
{

    public enum RepositoryEngine
    {
        Sql = 0,
        MongoDb = 1
    }

    public class Fabric
    {

        private RepositoryEngine repositoryType;

        public Fabric(RepositoryEngine type)
        {
            repositoryType = type;
        }

        #region Hierarchy.Model Instances
        public Hierarchy.Model.ITree NewTree() => new Budget.XAF.Model.Sql.Tree() as Hierarchy.Model.ITree;
        public Hierarchy.Model.ITreeName NewTreeName() => new Budget.XAF.Model.Sql.TreeName() as Hierarchy.Model.ITreeName;
        public Hierarchy.Model.INode NewNode() => new Budget.XAF.Model.Sql.Node() as Hierarchy.Model.INode;

        #endregion

        #region Hierarchy.Model SetPropertyValues

        public void SetTreeName(ref Hierarchy.Model.ITree tree,
                                Hierarchy.Model.ITreeName treeName)
        {
            if (tree == null) return;
            if (treeName == null) return;

            if (treeName is Budget.XAF.Model.Sql.TreeName &&
                tree is Budget.XAF.Model.Sql.Tree)
                (tree as Budget.XAF.Model.Sql.Tree).TreeName = treeName as Budget.XAF.Model.Sql.TreeName;
        }

        public void SetTree(ref Hierarchy.Model.INode node,
                              Hierarchy.Model.ITree tree)
        {
            if (node == null) return;
            if (tree == null) return;

            if (tree is Budget.XAF.Model.Sql.Tree &&
                node is Budget.XAF.Model.Sql.Node)
                (node as Budget.XAF.Model.Sql.Node).Tree = tree as Budget.XAF.Model.Sql.Tree;
        }

        #endregion       

    }
}