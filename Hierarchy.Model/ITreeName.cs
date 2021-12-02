namespace Hierarchy.Model
{
    public interface ITreeName
    {
        string Name { get; set; }
        ITree GetTree();
    }
}
